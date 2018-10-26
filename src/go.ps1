param(
    [string]
    $version="1.0.0.1",
    [string[]]
    # $include=@("*SystemEx*.nuspec","Numeric*.nuspec"),
    $include=@("*.nuspec"),
    [Parameter(ValueFromPipeline = $true)]
    [object[]]
    $nuspecFiles=(Get-ChildItem $PSScriptRoot -Include $include -Recurse),
    [string]
    $msbuild=$null,
    [string]
    $nugetApiKey=$null,
    [bool]
    $build=$true,
    [bool]
    $cleanBin=$true
)
Import-Module ..\tools\psake\psake.psm1
Invoke-psake .\Build.ps1 -properties @{
    "nuspecFiles"=$nuspecFiles;
    "version"=$version;
    "cleanBin"=(($include -eq "*.nuspec") -and $cleanBin);
    "msbuild"=$msbuild;
    "nugetApiKey"=$nugetApiKey;
    "build"=$build
}