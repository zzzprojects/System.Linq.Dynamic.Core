
#if !(NET35 || NET452 || DNX452 || DNXCORE50 || DOTNET5_4 || DOTNET5_1 || UAP10_0 || NETSTANDARD || PORTABLE)
using System.Threading.Tasks;
#endif

namespace System.Linq.Dynamic.Core.Tests
{
    public class InternalTests
    {
#if !(NET452 || NET35|| DNX452 || DNXCORE50 || DOTNET5_4 || DOTNET5_1 || UAP10_0 || NETSTANDARD || PORTABLE)
        [Fact]
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