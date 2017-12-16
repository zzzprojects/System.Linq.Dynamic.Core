using NFluent;
using System.Collections.Generic;
using System.Linq.Dynamic.Core.Tests.Entities;
using Xunit;
using User = System.Linq.Dynamic.Core.Tests.Helpers.Models.User;

namespace System.Linq.Dynamic.Core.Tests
{
    public class ComplexTests
    {
        public class Claim
        {
            public decimal? Balance { get; set; }
            public List<string> Tags { get; set; }
        }

        [Fact]
        // http://stackoverflow.com/questions/43272152/generate-dynamic-linq-query-using-outerit
        public void OuterIt_StackOverFlow_Question_43272152()
        {
            var claim1 = new Claim { Balance = 100, Tags = new List<string> { "Blah", "Blah Blah" } };
            var claim2 = new Claim { Balance = 500, Tags = new List<string> { "Dummy Tag", "Dummy tag 1", "New" } };

            var claims = new List<Claim> { claim1, claim2 };

            var tags = new List<string> { "New", "Blah" };
            var parameters = new List<object> { tags };
            var query = claims.AsQueryable().Where("Tags.Any(@0.Contains(it)) AND Balance > 100", parameters.ToArray()).ToArray();

            Check.That(query).ContainsExactly(claim2);
        }

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
    }
}
