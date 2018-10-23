Framework "4.6"
properties {
    $nugetExe="$PSScriptRoot\..\tools\Nuget.exe"
    $nugetBin="$PSScriptRoot\..\bin\nuget\"
    $version="2.0.0.0"
    $nuspecFiles=Get-ChildItem $PSScriptRoot *.nuspec -Recurse
    $msbuild=FindMSBuild
}

task ChangeAssemblyInfo {
    
}

task Compile {
    exec {
        & $msbuild .\PocketXaf.sln /fl
        if (! $?) { throw "compile failed" }
    }
}

task Clean{
    exec{
        & $msbuild .\PocketXaf.sln /t:Clean /fl
        if (! $?) { throw "clean failed" }
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
            $xml.SelectSingleNode("//version").InnerText=$version
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
        $nuspecFiles|ForEach-Object{
            & $nugetExe pack $_.FullName -version $version -OutputDirectory $nugetBin -Basepath $_.DirectoryName
        }
        
    }
}

task default  -depends Clean ,Compile ,UpdateNuspecMetadata,PackNuspec

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