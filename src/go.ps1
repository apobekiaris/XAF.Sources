param(
    [string]
    $version="1.0.0.0",
    $filter="*.nuspec",
    [Parameter(ValueFromPipeline = $true)]
    [object[]]
    $nuspecFiles=(Get-ChildItem $PSScriptRoot $filter -Recurse),
    [string]
    $msbuild=$null,
    [string]
    $nugetApiKey=$null
)
Import-Module ..\tools\psake\psake.psm1
Invoke-psake .\Build.ps1 -properties @{"nuspecFiles"=$nuspecFiles;"version"=$version;"cleanBin"=($filter -eq "*.nuspec");"msbuild"=$msbuild;"nugetApiKey"=$nugetApiKey}