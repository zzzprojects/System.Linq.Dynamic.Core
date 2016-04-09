using System.Linq.Dynamic.Core.Tests.Helpers;
#if DNXCORE50 || DNX451 || DNX452
using TestToolsToXunitProxy;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace System.Linq.Dynamic.Core.Tests
{
    [TestClass]
    public class OperatorTests
    {
        [TestMethod]
        
        public void Operator_Multiplication_Single_Float_ParseException()
        {
            //Arrange
            var models = new [] { new SimpleValuesModel() }.AsQueryable();

            //Act + Assert
            Helper.ExpectException<ParseException>(() => models.Select("FloatValue * DecimalValue"));
        }

        [TestMethod]
        public void Operator_Multiplication_Single_Float_Cast()
        {
            //Arrange
            var models = new SimpleValuesModel[] { new SimpleValuesModel() { FloatValue = 2, DecimalValue = 3 } }.AsQueryable();

            //Act
            var result = models.Select("Decimal(FloatValue) * DecimalValue").First();

            //Assert
            Assert.AreEqual(6.0m, result);
        }
    }
}
