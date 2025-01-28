rem https://github.com/StefH/GitHubReleaseNotes

SET version=v1.6.0.1

GitHubReleaseNotes --output CHANGELOG.md --exclude-labels out_of_scope invalid question documentation wontfix environment duplicate --language en --version %version% --token %GH_TOKEN%
