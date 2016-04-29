using Microsoft.Extensions.Logging;

namespace System.Linq.Dynamic.Core.Tests.Logging
{
    public class DbLogger : ILogger
    {
        public void Log(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            var message = $"{Environment.NewLine}{formatter(state, exception)}";

            if (message.ToLower().Contains("pi"))
                Console.WriteLine(message);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScopeImpl(object state)
        {
            return null;
        }
    }
}