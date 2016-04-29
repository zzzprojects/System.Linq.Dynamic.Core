using Microsoft.Extensions.Logging;

namespace System.Linq.Dynamic.Core.Tests.Logging
{
    public class DbLoggerProvider : ILoggerProvider
    {
        private static readonly string[] Categories =
        {
            typeof(Microsoft.Data.Entity.Storage.Internal.RelationalCommandBuilderFactory).FullName,
            //typeof(Microsoft.Data.Entity.Storage.Internal.SqliteRelationalConnection).FullName
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