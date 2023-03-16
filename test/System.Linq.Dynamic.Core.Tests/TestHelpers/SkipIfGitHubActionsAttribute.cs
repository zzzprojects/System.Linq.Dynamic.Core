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

    /// <summary>
    /// According to ChatGPT:
    /// 
    /// The GITHUB_ACTIONS environment variable is not officially documented by GitHub, but it is mentioned in the GitHub Actions documentation in a few places. Here are some relevant links:
    /// [Context and expression syntax for GitHub Actions](https://docs.github.com/en/actions/reference/context-and-expression-syntax-for-github-actions#github-context) - This page lists all the GitHub-specific context variables that are available in a GitHub Actions workflow, including GITHUB_ACTIONS.
    /// [Virtual environments for GitHub-hosted runners](https://docs.github.com/en/actions/reference/specifications-for-github-hosted-runners#virtual-environments-for-github-hosted-runners) - This page explains the environment variables that are available on GitHub-hosted runners, including GITHUB_ACTIONS.
    /// [Actions virtual environments](https://github.com/actions/virtual-environments#operating-systems) - This page provides more information about the virtual environments used by GitHub Actions, including the operating systems and software versions used on each environment.
    /// </summary>
    private static bool IsRunningOnGitHubActions()
    {
        return GetBool("GITHUB_ACTIONS") || GetBool("IsRunningOnGitHubActions");
    }

    private static bool GetBool(string variable)
    {
        return bool.TryParse(Environment.GetEnvironmentVariable(variable), out var value) && value;
    }
}