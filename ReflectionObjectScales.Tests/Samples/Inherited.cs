namespace ReflectionObjectScales.Tests.Samples
{
    public class Inherited : SingleInt
    {
        public new const int DebuggerSize = 32;

        public long L2 { get; set; }

        protected int I3 { get; set; } = 0x33;
    }
}