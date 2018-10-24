namespace System.Linq.Dynamic.Core.Parser
{
    public class WrappedValue<TValue>
    {
        public TValue Value { get; private set; }

        public WrappedValue(TValue value)
        {
            Value = value;
        }
    }
}
