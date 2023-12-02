using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using System.Linq.Expressions;

namespace System.Linq.Dynamic.Core.Tests
{
    public class StaticHelperSqlExpression
    {
        public Expression<Func<User, bool>>? Filter { get; set; }
    }
}