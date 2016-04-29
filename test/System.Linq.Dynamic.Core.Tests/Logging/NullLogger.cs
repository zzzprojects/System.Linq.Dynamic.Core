using Microsoft.Extensions.Logging;

namespace System.Linq.Dynamic.Core.Tests.Logging
{
    internal class NullLogger : ILogger
    {
        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        public void Log(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        { }

        public IDisposable BeginScopeImpl(object state)
        {
            return null;
        }
    }
}