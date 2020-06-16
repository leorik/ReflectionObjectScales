namespace ReflectionObjectScales.Tests.Samples
{
    public class Nested
    {
        public const int DebuggerSize = 24;

        public class Inside
        {
            public const int DebuggerSizeInside = 24;

            public long L1 { get; set; }
        }
    }
}