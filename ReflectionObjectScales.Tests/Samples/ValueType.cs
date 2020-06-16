namespace ReflectionObjectScales.Tests.Samples
{
    public struct ValueType
    {
        public const int DebuggerSize = 16;

        public long L { get; set; }

        public byte B { get; set; }

        public long Sum()
        {
            return L + B;
        }
    }
}