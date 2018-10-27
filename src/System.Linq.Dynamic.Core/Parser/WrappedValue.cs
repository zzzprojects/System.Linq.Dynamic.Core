namespace System.Linq.Dynamic.Core.Parser
{
    internal class WrappedValue<TValue>
    {
        public TValue Value { get; private set; }

        public WrappedValue(TValue value)
        {
            Value = value;
        }
    }
}
