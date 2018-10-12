Framework "4.6"
properties {
    $nugetExe="$PSScriptRoot\..\tools\Nuget.exe"
    $nugetBin="$PSScriptRoot\..\bin\nuget\"
    $version="2.0.0.0"
    $nuspecFiles=Get-ChildItem $PSScriptRoot *.nuspec -Recurse
}

task ChangeAssemblyInfo {
    
}

task Compile {
    exec {
        # & msbuild .\PocketXaf.sln /t:Clean /fl
        # if (! $?) { throw "msbuild failed" }
    }
}

task Clean{
    exec{
        # & msbuild .\PocketXaf.sln /fl
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

task default  -depends ShowMsBuildVersion ,Clean ,Compile ,UpdateNuspecMetadata,PackNuspec

task ShowMsBuildVersion {
    exec{
        msbuild /version
        Set-Location $PSScriptRoot
    }
}

task ? -Description "Helper to display task info" {
	Write-Documentation
}