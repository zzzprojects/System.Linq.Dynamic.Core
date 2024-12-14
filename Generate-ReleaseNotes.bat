rem https://github.com/StefH/GitHubReleaseNotes

SET version=v1.5.1

GitHubReleaseNotes --output CHANGELOG.md --exclude-labels invalid question documentation wontfix environment --language en --version %version% --token %GH_TOKEN%
