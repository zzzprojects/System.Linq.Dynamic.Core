using System.Collections.Generic;
using System.Linq.Dynamic.Core.Tests.Entities;
using System.Linq.Expressions;
using NFluent;
using Xunit;
using User = System.Linq.Dynamic.Core.Tests.Helpers.Models.User;

namespace System.Linq.Dynamic.Core.Tests
{
    public class ComplexTests
    {
        /// <summary>
        /// groupByExpressionX	"new (new (Company.Name as CompanyName) as GroupByFields)"	string
        /// </summary>
        [Fact]
        public void GroupByAndSelect_Test_Illegal_one_byte_branch_at_position_9_Requested_branch_was_143()
        {
            var testList = new List<Employee>();
            var qry = testList.AsQueryable();

            string keySelector = "new (new (Company.Name as CompanyName) as GroupByFields)";
            var group = qry.GroupBy(keySelector);
            Assert.NotNull(group);

            var result = group.ToDynamicList();
            Assert.NotNull(result);
        }

        /// <summary>
        /// groupByExpressionX	"new (new (Company.Name as CompanyName) as GroupByFields)"	string
        /// string.Format("new (it AS TEntity__{0})", includesX)    "new (it AS TEntity__, it.Company as TEntity__Company, it.Company.MainCompany as TEntity__Company_MainCompany, it.Country as TEntity__Country, it.Function as TEntity__Function, it.SubFunction as TEntity__SubFunction)"	string
        /// selectExpressionBeforeOrderByX = "new (Key.GroupByFields, it as Grouping , new (Count() as count__CompanyName, Min(TEntity__.EmployeeNumber) as min__Number, Max(TEntity__.EmployeeNumber) as max__Number, Average(TEntity__.EmployeeNumber) as average__Number, Sum(TEntity__.EmployeeNumber) as sum__Number) as Aggregates)";
        /// </summary>
        [Fact]
        public void GroupByAndSelect_Test_GroupByWithSelect()
        {
            var testList = new List<Employee>();
            var qry = testList.AsQueryable();

            string keySelector = "new (new (Company.Name as CompanyName) as GroupByFields)";
            string resultSelector = "new (it AS TEntity__, it.Company as TEntity__Company, it.Company.MainCompany as TEntity__Company_MainCompany, it.Country as TEntity__Country, it.Function as TEntity__Function, it.SubFunction as TEntity__SubFunction)";
            var group = qry.GroupBy(keySelector, resultSelector);
            Assert.NotNull(group);

            var result = group.ToDynamicList();
            Assert.NotNull(result);

            string selectExpressionBeforeOrderByX = "new (Key.GroupByFields, it as Grouping , new (Count() as count__CompanyName, Min(TEntity__.EmployeeNumber) as min__Number, Max(TEntity__.EmployeeNumber) as max__Number, Average(TEntity__.EmployeeNumber) as average__Number, Sum(TEntity__.EmployeeNumber) as sum__Number) as Aggregates)";
            var selectQ = group.Select(selectExpressionBeforeOrderByX);
            Assert.NotNull(selectQ);

            var resultSelect = selectQ.ToDynamicList();
            Assert.NotNull(resultSelect);
        }

        /// <summary>
        /// The purpose of this test is to verify that after a group by of a dynamically created
        /// key, the Select clause can access the key's members
        /// </summary>
        [Fact]
        public void GroupByAndSelect_TestDynamicSelectMember()
        {
            //Arrange
            var testList = User.GenerateSampleModels(51);
            var qry = testList.AsQueryable();

            //Act
            var byAgeReturnAllReal = qry.GroupBy(x => new { x.Profile.Age });
            var groupReal = byAgeReturnAllReal.OrderBy(gg => gg.Key.Age).ToList();

            var byAgeReturnAll = qry.GroupBy("new (Profile.Age)");

            var group = byAgeReturnAll.OrderBy("Key.Age").ToDynamicArray();

            var selectQry = byAgeReturnAll.Select("new (Key.Age, Sum(Income) As TotalIncome)");

            //Real Comparison
            var realQry = qry.GroupBy(x => new { x.Profile.Age }).Select(x => new { x.Key.Age, TotalIncome = x.Sum(y => y.Income) });

            //Assert
            Assert.Equal(realQry.Count(), selectQry.Count());
#if NET35
            Assert.Equal(
                realQry.Select(x => x.Age).ToArray(),
                selectQry.Cast<object>().Select(x => ((object)x).GetDynamicProperty<int?>("Age")).ToArray());

            Assert.Equal(
                realQry.Select(x => x.TotalIncome).ToArray(),
                selectQry.Cast<object>().Select(x => ((object)x).GetDynamicProperty<int>("TotalIncome")).ToArray());
#else
            Assert.Equal(
                realQry.Select(x => x.Age).ToArray(),
                selectQry.AsEnumerable().Select(x => x.Age).Cast<int?>().ToArray());

            Assert.Equal(
                realQry.Select(x => x.TotalIncome).ToArray(),
                selectQry.AsEnumerable().Select(x => x.TotalIncome).Cast<int>().ToArray());
#endif
        }

        private class ComplexParseLambda1Result
        {
            public int? Age;
            public int TotalIncome;
        }

        [Fact]
        public void ComplexParseLambda1()
        {
            //Arrange
            var testList = User.GenerateSampleModels(51);
            var qry = testList.AsQueryable();

            var externals = new Dictionary<string, object>
            {
                { "Users", qry }
            };

            string query = "Users.GroupBy(x => new { x.Profile.Age }).OrderBy(gg => gg.Key.Age).Select(j => new (j.Key.Age, j.Sum(k => k.Income) As TotalIncome))";
            LambdaExpression expression = DynamicExpressionParser.ParseLambda(null, query, externals);
            Delegate del = expression.Compile();
            IEnumerable<dynamic> result = del.DynamicInvoke() as IEnumerable<dynamic>;

            var expected = qry.GroupBy(x => new { x.Profile.Age }).OrderBy(gg => gg.Key.Age).Select(j => new { j.Key.Age, TotalIncome = j.Sum(k => k.Income) }).Select(c => new ComplexParseLambda1Result { Age = c.Age, TotalIncome = c.TotalIncome }).Cast<dynamic>().ToArray();

            Check.That(result).IsNotNull();
            Check.That(result).HasSize(expected.Length);
            Check.That(result.ToArray()[0]).Equals(expected[0]);
        }

        [Fact]
        public void ComplexParseLambda2()
        {
            //Arrange
            var testList = User.GenerateSampleModels(51);
            var qry = testList.AsQueryable();

            var externals = new Dictionary<string, object>
            {
                {"Users", qry}
            };

            string query = "Users.Select(j => new User(j.Income As Income))";
            LambdaExpression expression = DynamicExpressionParser.ParseLambda(null, query, externals);
            Delegate del = expression.Compile();
            object result = del.DynamicInvoke();

            Assert.NotNull(result);
        }
    }
}