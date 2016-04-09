using System.Globalization;
using JetBrains.Annotations;

// copied from https://github.com/aspnet/EntityFramework/blob/dev/src/Microsoft.EntityFrameworkCore/Properties/CoreStrings.resx
namespace System.Linq.Dynamic.Core.Validation
{
    internal static class CoreStrings
    {
        /// <summary>
        /// The property '{property}' of the argument '{argument}' cannot be null.
        /// </summary>
        public static string ArgumentPropertyNull([CanBeNull] string property, [CanBeNull] string argument)
        {
            return string.Format(CultureInfo.CurrentCulture, $"The property '{property}' of the argument '{argument}' cannot be null.", property, argument);
        }

        /// <summary>
        /// The string argument '{argumentName}' cannot be empty.
        /// </summary>
        public static string ArgumentIsEmpty([CanBeNull] string argumentName)
        {
            return string.Format(CultureInfo.CurrentCulture, $"The string argument '{argumentName}' cannot be empty.", argumentName);
        }

        /// <summary>
        /// The entity type '{type}' provided for the argument '{argumentName}' must be a reference type.
        /// </summary>
        public static string InvalidEntityType([CanBeNull] Type type, [CanBeNull] string argumentName)
        {
            return string.Format(CultureInfo.CurrentCulture, $"The entity type '{type}' provided for the argument '{argumentName}' must be a reference type.", type, argumentName);
        }

        /// <summary>
        /// The collection argument '{argumentName}' must contain at least one element.
        /// </summary>
        public static string CollectionArgumentIsEmpty([CanBeNull] string argumentName)
        {
            return string.Format(CultureInfo.CurrentCulture, $"The collection argument '{argumentName}' must contain at least one element.", argumentName);
        }
    }
}