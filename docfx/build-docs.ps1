<#
.SYNOPSIS
    Builds documentation for this project into static HTML output

.NOTES
    Author: Stef Heyenrath - @sheyenrath

.LINK
    https://github.com/StefH/System.Linq.Dynamic.Core/docfx
	  https://raw.githubusercontent.com/jsiegmund/docfx-seed/master/build-docs.ps1
#>

# For running this script: "Set-ExecutionPolicy Bypass"

param (
  [Switch]$Serve = $false
)

Write-Host "Starting docfx build process." -ForegroundColor Yellow

$docfxArgs = @(    
  "docfx.json"
)

# Add '--serve' to arguments in order to start serving the site in place
if ($Serve.IsPresent) {
  $docfxArgs = $docfxArgs + "--serve"
}

# Start the make process which uses Sphynx to convert RST to HTML
Start-Process "docfx" -ArgumentList $docfxArgs -NoNewWindow -Wait