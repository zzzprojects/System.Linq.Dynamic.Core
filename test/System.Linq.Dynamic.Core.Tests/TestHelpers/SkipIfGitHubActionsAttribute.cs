using Xunit;

namespace System.Linq.Dynamic.Core.Tests.TestHelpers;

internal class SkipIfGitHubActionsAttribute : FactAttribute
{
    public SkipIfGitHubActionsAttribute()
    {
        if (IsRunningOnGitHubActions())
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            Skip = "This test is skipped on GitHub Actions";
        }
    }

    private static bool IsRunningOnGitHubActions()
    {
        return GetBool("GITHUB_ACTIONS") || GetBool("IsRunningOnGitHubActions");
    }

    private static bool GetBool(string variable)
    {
        return bool.TryParse(Environment.GetEnvironmentVariable(variable), out var value) && value;
    }
}