#if EFCORE
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.Extensions.Logging;

#if EFCORE_3X
using Microsoft.EntityFrameworkCore.Storage;
#endif

namespace System.Linq.Dynamic.Core.Tests.Logging
{
    public class DbLoggerProvider : ILoggerProvider
    {
        private static readonly string[] Categories =
        {
            typeof(RelationalCommandBuilderFactory).FullName,
            // typeof(SqliteRelationalConnection).FullName
        };

        public ILogger CreateLogger(string name)
        {
            if (Categories.Contains(name))
            {
                return new DbLogger();
            }

            return new NullLogger();
        }

        public void Dispose()
        {
        }
    }
}
#endif
