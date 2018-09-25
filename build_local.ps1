
param(
    [string]$nuget_path= $("C:\nuget")
    )
    
	wget "https://raw.githubusercontent.com/rducom/ALM/master/build/ComputeVersion.ps1" -outfile "ComputeVersion.ps1"
    . .\ComputeVersion.ps1 
    
    $version = Compute .\src\MerQure\MerQure.csproj
    $props = "/p:Configuration=Debug,VersionPrefix="+($version.Prefix)+",VersionSuffix="+($version.Suffix)
    $propack = "/p:PackageVersion="+($version.Semver) 
    Write-Host $props
    Write-Host $propack

    dotnet restore
    dotnet build .\MerQure.sln $props
    dotnet pack .\src\MerQure\MerQure.csproj --configuration Debug $propack -o $nuget_path
    dotnet pack .\src\MerQure.RbMQ\MerQure.RbMQ.csproj --configuration Debug $propack -o $nuget_path
    dotnet pack .\src\MerQure.RbMQ\MerQure.Tools.csproj --configuration Debug $propack -o $nuget_path