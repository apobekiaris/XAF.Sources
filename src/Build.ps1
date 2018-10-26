Framework "4.6"
. ..\tools\Invoke-InParallel.ps1
properties {
    $nugetExe="$PSScriptRoot\..\tools\Nuget.exe"
    $nugetBin="$PSScriptRoot\..\bin\nuget\"
    $version=$null
    $nuspecFiles=$null
    $msbuild=$null
    $cleanBin=$null
    $nugetApiKey=$null
    $build=$true
}

task ChangeAssemblyInfo {
    
}

task Compile -precondition {return $build} {
    exec {
        & $script:msbuild .\Xaf.sln /t:Clean /fl
        if (! $?) { throw "clean failed" }
        & $script:msbuild .\Xaf.sln /fl
        if (! $?) { throw "compile failed" }
    }
}

task Clean -precondition {return $cleanBin}{
    exec{
        if (Test-Path $nugetBin){
            Remove-Item $nugetBin -Force -Recurse
        }
    }
}

Task VersionDependencies{
    Exec{
        $nuspecFiles | ForEach-Object{
            $xml=[xml](Get-Content -path $_.FullName)
            write-host "Versioning $($_.FullName)"
            $xml.SelectNodes("//dependency")|foreach{
                $versionAttribute=$_.Attributes["version"]
                if ($versionAttribute){
                    if ($versionAttribute.Value -eq '$version'){
                        $versionAttribute.Value=$version
                    }
                }
            }
            $description=$xml.SelectSingleNode("//description").InnerText
            if ($description -eq $null){
                $xml.SelectSingleNode("//description").InnerText=$_   
            }
            $xml.Save($_.FullName)
        }
    }
}
Task  UpdateNuspecMetadata -depends VersionDependencies {
    exec{
        $nuspecFiles | ForEach-Object{
            $xml=[xml](Get-Content -path $_.FullName)
            $xml.SelectSingleNode("//licenseUrl").InnerText="https://github.com/eXpandFramework/XAF/blob/master/LICENSE"
            $xml.SelectSingleNode("//projectUrl").InnerText="https://github.com/eXpandFramework/XAF"
            $description=$xml.SelectSingleNode("//description").InnerText
            if ($description -eq $null){
                $xml.SelectSingleNode("//description").InnerText=$_   
            }
            $xml.Save($_.FullName)
        }
    }
}

Task PackNuspec{
    Exec{
        New-Item $nugetBin -ItemType Directory -Force
        $paramObject = [pscustomobject] @{
            version=$version
            nugetBin=$nugetBin
            nugetExe=$nugetExe
        }
        Invoke-InParallel -InputObject $nuspecFiles -Parameter $paramObject -runspaceTimeout 30  -ScriptBlock {  
            Write-Host "a=$($_.FullName)"
            & $parameter.nugetExe pack $_.FullName -version $parameter.version -OutputDirectory $parameter.nugetBin -Basepath $_.DirectoryName
        }
    }
}
Task DiscoverMSBuild{
    Exec{
        Write-Host "msbuild=$msbuild"
        if (!$msbuild){
            $script:msbuild=(FindMSBuild)
        }
        else{
            $script:msbuild=$msbuild
        }
    }
}
Task PublishNuget -precondition{return $nugetApiKey}{
    Exec{
        Get-ChildItem -Path $nugetBin -Filter *.nupkg|foreach{
            & $nugetExe push $_.FullName $nugetApiKey -source https://api.nuget.org/v3/index.json
        }
    }
}

task default  -depends DiscoverMSBuild,Clean ,Compile ,UpdateNuspecMetadata,PackNuspec,PublishNuget

function FindMSBuild() {
    if (!(Get-Module -ListAvailable -Name VSSetup)) {
        Write-Host "Module exists"
        Set-PSRepository -Name "PSGallery" -InstallationPolicy Trusted
        Install-Module VSSetup
    }
    $path=Get-VSSetupInstance  | Select-VSSetupInstance -Product Microsoft.VisualStudio.Product.BuildTools -Latest |Select-Object -ExpandProperty InstallationPath
    return join-path $path MSBuild\15.0\Bin\MSBuild.exe
}

task ? -Description "Helper to display task info" {
	Write-Documentation
}