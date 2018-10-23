param(
    [string]
    $version="2.0.0.0",
    $filter="*.nuspec",
    [Parameter(ValueFromPipeline = $true)]
    [object[]]
    $nuspecFiles=(Get-ChildItem $PSScriptRoot $filter -Recurse)
)
Import-Module ..\tools\psake\psake.psm1
Invoke-psake .\Build.ps1 -properties @{"nuspecFiles"=$nuspecFiles;"version"=$version;"cleanBin"=($filter -eq "*.nuspec")}