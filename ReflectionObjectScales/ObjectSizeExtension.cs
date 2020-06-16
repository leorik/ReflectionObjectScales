using System;
using System.Reflection;

namespace ReflectionObjectScales
{
    /// <summary>
    ///     Extension methods for <see cref="object" /> for approximate calculation of runtime memory footprint.
    ///     Object layout and inheritance info comes from reflection, so it might not work with AOT and might be pretty slow.
    /// </summary>
    public static class ObjectSizeExtension
    {
        /// <summary>
        ///     Size threshold for object to be put in large object heap (LOH), in bytes.
        /// </summary>
        private const int LohThreshold = 85_000;

        /// <summary>
        ///     Objects in LOH are always aligned to 8 bytes.
        /// </summary>
        private const int LohAlignment = 7;

        /// <summary>
        ///     Memory footprint of managed reference.
        /// </summary>
        private static int ReferenceSize => IntPtr.Size;

        /// <summary>
        ///     Size for CLR service fields (ObjHeader + MethodTable pointer) for each object.
        /// </summary>
        private static int ObjectServiceFilesSize => ReferenceSize * 2;

        /// <summary>
        ///     Minimal size of managed object.
        /// </summary>
        private static int MinObjSize => ReferenceSize * 3; // TODO crosscheck for 32-bit systems

        /// <summary>
        ///     Calculates size of managed object based on its contents.
        ///     Treats references for other objects in heap as pointer-width fields, thus making calculated size exclusive.
        /// </summary>
        /// <param name="obj">Managed object to calculate size of.</param>
        /// <returns>Approximate size of managed object memory consumption.</returns>
        /// <exception cref="ArgumentNullException">If provided object is null.</exception>
        public static int GetExclusiveSize(this object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            var type = obj.GetType();
            if (IsBasicType(type)) return GetBasicTypeSize(type);

            if (type == typeof(string)) return GetStringSize((string) obj);

            int objectSize;
            if (type.IsArray)
            {
                objectSize = AlignSize(CalculateArrayContentsSize(obj, type) + sizeof(int)); // Content + Length
            }
            else
            {
                objectSize = CalculateFieldsSize(type, !type.IsEnum);

                if (type.IsValueType) return objectSize;
            }

            objectSize += ObjectServiceFilesSize; // ObjHeader + MethodTable

            objectSize = objectSize > MinObjSize ? objectSize : MinObjSize;

            return objectSize;
        }

        /// <summary>
        ///     Calculates string memory footprint.
        /// </summary>
        /// <param name="str">String to calculate size of.</param>
        /// <returns>Memory size of string in bytes.</returns>
        private static int GetStringSize(string str)
        {
            return str.Length * sizeof(char) // String content
                   + sizeof(int) // String length field
                   + sizeof(char) // Null terminator
                   + ObjectServiceFilesSize; // ObjHeader + MethodTable Refs
        }

        /// <summary>
        ///     Calculates size of array.
        /// </summary>
        /// <param name="array">Value of array.</param>
        /// <param name="arrayType">Type of array.</param>
        /// <returns>Memory size of array in bytes.</returns>
        private static int CalculateArrayContentsSize(object array, Type arrayType)
        {
            int arrayElementSize;
            var arrayElementType = arrayType.GetElementType();
            // Impossible to be null since we explicitly check for given to be array in calling method.
            // ReSharper disable once PossibleNullReferenceException
            if (IsBasicType(arrayElementType))
                arrayElementSize = GetBasicTypeSize(arrayElementType);
            else if (arrayElementType.IsValueType)
                arrayElementSize = CalculateFieldsSize(arrayElementType);
            else
                arrayElementSize = ReferenceSize;

            var elementsCount = ((Array) array).Length;

            return elementsCount * arrayElementSize;
        }

        /// <summary>
        ///     Accumulates sizes of all fields of given type and its parents.
        /// </summary>
        /// <param name="type">Type to determine declared fields.</param>
        /// <param name="shouldAlign">Should we apply alignment to end result?</param>
        /// <returns>Size of all declared fields of given type and its parents, in bytes.</returns>
        private static int CalculateFieldsSize(Type type, bool shouldAlign = true)
        {
            var fieldsSize = 0;

            while (ShouldFollowOnType(type))
            {
                // Impossible for type to be null since we check it for null in ShouldFollowOnType.
                // ReSharper disable once PossibleNullReferenceException
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                                            BindingFlags.DeclaredOnly);
                foreach (var field in fields) fieldsSize += CalculateFieldSize(field);

                type = type.BaseType;
            }

            if (shouldAlign) fieldsSize = AlignSize(fieldsSize);

            return fieldsSize;
        }

        /// <summary>
        ///     Determines whenever given type should be accounted for size calculation.
        /// </summary>
        /// <param name="type">Type to be checked.</param>
        /// <returns>Should we calculate size for fields of given type?</returns>
        private static bool ShouldFollowOnType(Type type)
        {
            return type != null && type != typeof(object) && type != typeof(ValueType);
        }

        /// <summary>
        ///     Calculates size of single field.
        /// </summary>
        /// <param name="fieldInfo">Information about field.</param>
        /// <returns>Size of provided field.</returns>
        private static int CalculateFieldSize(FieldInfo fieldInfo)
        {
            var fieldType = fieldInfo.FieldType;
            if (IsBasicType(fieldType)) return GetBasicTypeSize(fieldType);

            return fieldType.IsValueType
                ? CalculateFieldsSize(fieldType, !fieldType.IsEnum) // Structs get embedded into object in memory,
                // so we want to recursively calculate size of struct
                : ReferenceSize;
        }

        /// <summary>
        ///     Applies alignment to size.
        /// </summary>
        /// <param name="fieldsSize">Pre-aligned size.</param>
        /// <returns>Aligned size.</returns>
        private static int AlignSize(int fieldsSize)
        {
            var alignmentShadowSize = fieldsSize < LohThreshold ? ReferenceSize - 1 : LohAlignment;

            fieldsSize = (fieldsSize + alignmentShadowSize) & ~alignmentShadowSize;

            return fieldsSize;
        }

        /// <summary>
        ///     Determines if given type is one of basic types.
        ///     See https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/built-in-types
        /// </summary>
        /// <param name="type">Suspected basic type.</param>
        /// <returns>Is given type one of built-in?</returns>
        private static bool IsBasicType(Type type)
        {
            return type.IsPrimitive || type == typeof(decimal);
        }

        /// <summary>
        ///     Extracts size for given basic type.
        /// </summary>
        /// <param name="type">Basic type.</param>
        /// <returns>Size of given type in bytes.</returns>
        /// <remarks>
        ///     Unfortunately, while CIL has opcode `sizeof`
        ///     (https://docs.microsoft.com/en-US/dotnet/api/system.reflection.emit.opcodes.sizeof?view=netcore-3.1),
        ///     that can operate on <see cref="Type" />, C# operator `sizeof` can't.
        ///     So we either forced to manually check for each individual built-in type,
        ///     or roll out more eloquent but also more hacky IL generation in runtime using
        ///     <see cref="System.Reflection.Emit.ILGenerator" />.
        /// </remarks>
        /// <exception cref="InvalidOperationException">If given type is not basic.</exception>
        private static int GetBasicTypeSize(Type type)
        {
            // Yup, it ain't pretty.
            if (type == typeof(int)) return sizeof(int);

            if (type == typeof(bool)) return sizeof(bool);

            if (type == typeof(byte)) return sizeof(byte);

            if (type == typeof(sbyte)) return sizeof(sbyte);

            if (type == typeof(char)) return sizeof(char);

            if (type == typeof(decimal)) return sizeof(decimal);

            if (type == typeof(double)) return sizeof(double);

            if (type == typeof(float)) return sizeof(float);

            if (type == typeof(uint)) return sizeof(uint);

            if (type == typeof(long)) return sizeof(long);

            if (type == typeof(ulong)) return sizeof(ulong);

            if (type == typeof(short)) return sizeof(short);

            if (type == typeof(ushort)) return sizeof(ushort);

            throw new InvalidOperationException($"Type {type.Name} is not basic.");
        }
    }
}