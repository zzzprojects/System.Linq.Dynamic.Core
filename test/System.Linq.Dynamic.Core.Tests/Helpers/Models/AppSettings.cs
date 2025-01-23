using System.Collections.Generic;
using System.Linq.Dynamic.Core.CustomTypeProviders;

namespace System.Linq.Dynamic.Core.Tests.Helpers.Models
{
    public static class AppSettings
    {
        public static Dictionary<string, string> SettingsProp { get; } = new()
        {
            { "jwt", "test" }
        };

        public static Dictionary<string, string> SettingsField = new()
        {
            { "jwt", "test" }
        };

        public const string ConstantField = "test";
    }

    [DynamicLinqType]
    public static class AppSettings2
    {
        public static Dictionary<string, string> SettingsProp { get; } = new()
        {
            { "jwt", "test" }
        };

        public static Dictionary<string, string> SettingsField = new()
        {
            { "jwt", "test" }
        };

        public const string ConstantField = "test";
    }

    public class AppSettings3
    {
        public static Dictionary<string, string> SettingsProp { get; } = new()
        {
            { "jwt", "test" }
        };

        public static Dictionary<string, string> SettingsField = new()
        {
            { "jwt", "test" }
        };

        public const string ConstantField = "test";
    }
}