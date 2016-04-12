#if DNXCORE50 || DNX451 || DNX452
using System.Reflection;
using TestToolsToXunitProxy;
using ReflectionBridge;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace System.Linq.Dynamic.Core.Tests.Helpers
{
    static class Helper
    {
        public static void ExpectException<TException>(Action action) where TException : Exception
        {
            Exception ex = null;

            try
            {
                action();
            }
            catch (TException exception)
            {
                if (exception.GetType() == typeof(TException)) return;

                ex = exception;
            }
            catch (Exception exception)
            {
                ex = exception;
            }

            Assert.Fail("Expected Exception did not occur.");
        }
    }
}