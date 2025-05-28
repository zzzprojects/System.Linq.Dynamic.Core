rem https://github.com/StefH/GitHubReleaseNotes

SET version=v1.6.5

GitHubReleaseNotes --output CHANGELOG.md --exclude-labels known_issue out_of_scope not_planned invalid question documentation wontfix environment duplicate --language en --version %version% --token %GH_TOKEN%
