rem https://github.com/StefH/GitHubReleaseNotes

SET version=1.2.14

GitHubReleaseNotes --output CHANGELOG.md --exclude-labels invalid question documentation wontfix --language en --version %version% --token %GH_TOKEN%
