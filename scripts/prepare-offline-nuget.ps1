[CmdletBinding()]
param(
    [string] $Version = '1.0.4',
    [string] $Destination = (Join-Path $PSScriptRoot '..\packages\nuget-offline')
)

$ErrorActionPreference = 'Stop'
$packageCache = Join-Path $env:USERPROFILE '.nuget\packages'
$destinationPath = [System.IO.Path]::GetFullPath($Destination)

if (-not (Test-Path -LiteralPath $packageCache -PathType Container)) {
    throw "NuGet global-packages folder was not found: $packageCache"
}

New-Item -ItemType Directory -Force -Path $destinationPath | Out-Null

$packages = Get-ChildItem -LiteralPath $packageCache -Directory -Filter 'pertamina.*' |
    ForEach-Object {
        Get-ChildItem -LiteralPath (Join-Path $_.FullName $Version) -File -Filter '*.nupkg' -ErrorAction SilentlyContinue
    }

if (-not $packages) {
    throw "No Pertamina.* NuGet packages version $Version were found in $packageCache. Run dotnet restore on a connected Windows machine first."
}

$packages | Copy-Item -Destination $destinationPath -Force

Write-Host "Copied $($packages.Count) Pertamina package(s) version $Version to $destinationPath"
