using System.Reflection;
using Xunit.Sdk;

namespace System.Linq.Dynamic.Core.Tests.TestHelpers
{
#if !NET452
    class DisplayTestMethodNameAttribute : BeforeAfterTestAttribute
    {
        public override void Before(MethodInfo methodUnderTest)
        {
            Console.WriteLine("Setup for test '{0}.'", methodUnderTest.Name);
        }

        public override void After(MethodInfo methodUnderTest)
        {
            Console.WriteLine("TearDown for test '{0}.'", methodUnderTest.Name);
        }
    }
#endif
}