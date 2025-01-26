rem https://github.com/StefH/GitHubReleaseNotes

SET version=v1.6.0

GitHubReleaseNotes --output CHANGELOG.md --exclude-labels invalid question documentation wontfix environment duplicate --language en --version %version% --token %GH_TOKEN%
