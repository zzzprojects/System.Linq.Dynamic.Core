using System.Threading;

#if NET35
namespace System
{
    internal class Lazy<T>
    {
        private readonly Func<T> _valueFactory;
        private readonly object _lock;

        private T? _value;
        private bool _valueCreated;

        public Lazy(Func<T> valueFactory, LazyThreadSafetyMode mode)
        {
            _valueFactory = valueFactory;
            _lock = new object();
        }

        public T Value
        {
            get
            {
                lock (_lock)
                {
                    if (_valueCreated)
                    {
                        return _value!;
                    }

                    _value = _valueFactory();
                    _valueCreated = true;
                    return _value;
                }
            }
        }

    }
}
#endif