rem https://github.com/StefH/GitHubReleaseNotes

SET version=v1.3.2-preview-01

GitHubReleaseNotes --output CHANGELOG.md --exclude-labels invalid question documentation wontfix --language en --version %version% --token %GH_TOKEN%
