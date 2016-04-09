#if DNXCORE50 || DNX451 || DNX452
using TestToolsToXunitProxy;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

#if !(NET35|| DNX452 || DNXCORE50)
using System.Threading.Tasks;
#endif

namespace System.Linq.Dynamic.Core.Tests
{
    [TestClass]
    public class InternalTests
    {
#if !(NET35|| DNX452 || DNXCORE50)
        [TestMethod]
        public void ClassFactory_LoadTest()
        {
            //Arrange
            var rnd = new Random(1);

            var testPropertiesGroups = new DynamicProperty[][] {
                new DynamicProperty[] { 
                    new DynamicProperty("String1", typeof( string )), 
                },
                new DynamicProperty[] { 
                    new DynamicProperty("String1", typeof( string )), 
                    new DynamicProperty("String2", typeof( string )) 
                },
                new DynamicProperty[] { 
                    new DynamicProperty("String1", typeof( string )), 
                    new DynamicProperty("Int1", typeof( int )) 
                },
                new DynamicProperty[] { 
                    new DynamicProperty("Int1", typeof( int )), 
                    new DynamicProperty("Int2", typeof( int )) 
                },
                new DynamicProperty[] { 
                    new DynamicProperty("String1", typeof( string )), 
                    new DynamicProperty("String2", typeof( string )), 
                    new DynamicProperty("String3", typeof( string )), 
                },
            };

            Action<int> testActionSingle = i =>
            {
                ClassFactory.Instance.GetDynamicClass(testPropertiesGroups[0]);
            };

            Action<int> testActionMultiple = i => {
                var testProperties = testPropertiesGroups[rnd.Next(0, testPropertiesGroups.Length)];

                ClassFactory.Instance.GetDynamicClass(testProperties);
            };

            //Act
            Parallel.For(0, 100000, testActionSingle);

            Parallel.For(0, 100000, testActionMultiple);

        }
#endif

    }
}