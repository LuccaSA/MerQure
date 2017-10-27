
	# 1.2.3-dev-Z 			-> local dev build 								[local]   (Z = local counter) 
	# 1.2.3-PR4824-X 		-> Pull Request build 							[private] (X = remote counter)
	# 1.2.3-beta-X  		-> build on master, without commit tag 			[private] (X = remote counter) {last tag = 1.2.3}
	# 1.2.3-rc3-beta-X  	-> build on master, without commit tag		 	[private] (X = remote counter) {last tag = 1.2.3-rc3}

	# 1.2.3-rc3  			-> build on master, with git Tag = "1.2.3-rc3"  [public]  [pre-release]
	# 1.2.4  				-> build on master, with git Tag = "1.2.3" 		[public]  [release]
	function Compute (
		[Parameter(Mandatory=$true)][string]$CsProjPath,
		[Parameter(Mandatory=$false)][string]$Counter,
        [Parameter(Mandatory=$false)][string]$IsTag,
        [Parameter(Mandatory=$false)][string]$PullRequest
	)
	{
		[version]$NearestVersion
		[string]$RequiredSuffix
		# Try extract git tag
		try{
			$tagfound = git.exe describe --tags $(git.exe rev-list --tags --max-count=1)
			$pre = GetPrefix($tagfound.Trim())
			$RequiredSuffix = GetSuffix($tagfound)
			$NearestVersion = [version]::Parse($pre)
			Write-Host "Git tag found : " $tagfound
			#if there's a git tag in the past, it overrides the csproj version
		}
		catch{ 
			Write-Host "No git tag found"
		} 
	
		if(!$NearestVersion){
			#fallback on try csproj version extraction
			try{
				# Try find VersionPrefix in new csproj format
				$xmlFound = Select-Xml -XPath "/Project/PropertyGroup/VersionPrefix" -Path $CsProjPath
				if(![string]::IsNullOrEmpty($xmlFound.Node.InnerText)){
					$pre = GetPrefix($xmlFound.Node.InnerText) 
					$NearestVersion = [version]::Parse($pre)
					Write-Host "Csproj VersionPrefix found : " $NearestVersion 
				}else{
					# fallback on Version 
					$xmlFound = Select-Xml -XPath "/Project/PropertyGroup/Version" -Path $CsProjPath
					if(![string]::IsNullOrEmpty($xmlFound.Node.InnerText)){
						$pre = GetPrefix($xmlFound.Node.InnerText)
						$RequiredSuffix = GetSuffix($xmlFound.Node.InnerText)
						$NearestVersion = [version]::Parse($pre)
						Write-Host "Csproj Version found : " $NearestVersion
					}else{
						throw "nothing"
					}
				} 
			}catch{
				Write-Host "No Csproj version found"
			}
		}

		$NearestVersion = VersionNormed($NearestVersion)
		
		#If no CI build counter, use local counter
		[int]$RevisionCounter = 0
		[bool]$ModeLocal = $false
		$file = ".\.versionCounter"
		if([string]::IsNullOrEmpty($Counter)){
			# in local mode, we use a local file to store the last build
			$ModeLocal = $true
			if(Test-path $file){
				try{
					$RevisionCounter = [int](Get-Content $file | Select -First 1) 
					Write-Host "local counter found : " $RevisionCounter
				}catch{
					Write-Host "Error parsing local counter"
				}
			}else{
				Write-Host "No '.versionCounter' file"
			}
		}else{
			# in remote mode, the CI system provides the counter value
			Write-Host "Remote counter found : " $Counter
			$RevisionCounter = $Counter
		}

		# ------------- Version construction -----------------------
	
		# Version is build from prefix and the build counter.
		# 1.2.3.X 
		$Version = "{0}.{1}.{2}.{3}" -f $NearestVersion.Major, $NearestVersion.Minor, $NearestVersion.Build, $RevisionCounter
		# Prefix is guessed from Git Tag or existing <Version/> or <VersionPrefix/> in csproj
		# 1.2.3 
		$VersionPrefix = "{0}.{1}.{2}" -f $NearestVersion.Major, $NearestVersion.Minor, $NearestVersion.Build 

		# Suffix is alway computed, from the context parameters (ex: beta-X)
		# EXCEPT : when the suffix comes from the git tag (ex 1.2.3-rc42). In this case
		$VersionSuffix
		
		if(![string]::IsNullOrEmpty($IsTag) -and [System.Convert]::ToBoolean($IsTag)){
			# git tag mode : either release or pre-release
			$deploy_public = $true
			if (![string]::IsNullOrEmpty($RequiredSuffix)){	
				Write-Host "Mode : pre-release" 	# tag like 1.2.3-rc3 => mode pre-release
				$VersionSuffix = $RequiredSuffix
			}else{									
				Write-Host "Mode : release" 		# tag like 1.2.3 => mode release
				$VersionSuffix = ""
			}
		}else{
			if($ModeLocal){			
				$deploy_local = $true			
				Write-Host "Mode : dev" 						# 1.2.3-dev-X
				$VersionSuffix = "dev-" + $RevisionCounter
			}elseif(![string]::IsNullOrEmpty($PullRequest)){ 
				$deploy_unstable = $true
				Write-Host "Mode : pull-request (alpha)" 		# mode on PR => # 1.2.3-PR4824-X 
				$VersionSuffix = "PR" + $PullRequest + "-" + $RevisionCounter
			}elseif (![string]::IsNullOrEmpty($RequiredSuffix)){
				$deploy_unstable = $true
				Write-Host "Mode : master beta pre-release" 				# mode build on master => 1.2.3-rc3-X
				$VersionSuffix = $RequiredSuffix + "-" + $RevisionCounter
			}else{
				$deploy_unstable = $true
				Write-Host "Mode : master beta release" 				# mode build on master => 1.2.3-beta-X 
				$VersionSuffix = "beta-" + $RevisionCounter
			}
		}
		
		if($deploy_public){
			$env:deploy_public = "true"
			Write-Host "Deploy : public"
		}
		if($deploy_unstable){
			$env:deploy_unstable = "true"
			Write-Host "Deploy : private"
		}
		if($deploy_local){
			Write-Host "Deploy : local"
		}

		if($ModeLocal){
			Write-Host "local counter incrementation"
			($RevisionCounter + 1) | Set-Content $file
		}
	
		[string]$Semver
		if ([string]::IsNullOrWhiteSpace($VersionSuffix)){
			$Semver = $VersionPrefix
		}else{
			$Semver = $VersionPrefix + "-" + $VersionSuffix
		}
		
		Write-Host "Nearest  = " $NearestVersion
		Write-Host "Semver   = " $Semver
		Write-Host "Assembly = " $Version 

		return New-Object PSObject -Property @{
			Nearest = $NearestVersion
			Semver = $Semver
			Prefix = $VersionPrefix
			Suffix = $VersionSuffix
			Assembly = $Version 
		}
	}
	
	function GetPrefix ([string] $v = $(throw "version is a required parameter.")) 
	{
		$v = $v.TrimStart("v")
		if($v.IndexOf('-') -gt 0){
			return $v.Substring(0,$v.IndexOf('-'))
		}else{
			return $v
		}
	}

	function GetSuffix ([string] $v = $(throw "version is a required parameter.")) 
	{
		$v = $v.TrimStart("v")
		if($v.IndexOf('-') -gt 0){
			return $v.Substring($v.IndexOf('-') + 1)
		}else{
			return $null
		}
	}

	function VersionNormed ([Version] $v = $(throw "version is a required parameter.")) 
	{
		$Major, $Minor, $Build, $Revision = 0
		If ($v.Major -gt 0){ $Major = $v.Major } 
		If ($v.Minor -gt 0){ $Minor = $v.Minor }
		If ($v.Build -gt 0){ $Build = $v.Build }
		If ($v.Revision -gt 0){ $Revision = $v.Revision }
		return [version]::new($Major,$Minor,$Build,$Revision)
	}