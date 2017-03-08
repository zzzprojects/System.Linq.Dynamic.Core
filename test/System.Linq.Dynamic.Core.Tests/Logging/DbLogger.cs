#if EFCORE
using Microsoft.Extensions.Logging;

namespace System.Linq.Dynamic.Core.Tests.Logging
{
    public class DbLogger : ILogger
    {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var message = $"{Environment.NewLine}{formatter(state, exception)}";

            if (message.ToLower().Contains("pi"))
                Console.WriteLine(message);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}
#endif