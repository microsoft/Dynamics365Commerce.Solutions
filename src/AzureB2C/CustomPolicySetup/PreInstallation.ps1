﻿  param (
 [Parameter(Mandatory=$True, Position=0, ValueFromPipeline=$false)]
 [System.String]
 $TenantId)

 if ($TenantId -eq "")
  {
    Write-Host "TenantId parameter is missing."
    Exit
  }

  $TenantId = $TenantId -replace ".onmicrosoft.com" -replace ""

# DO NOT MODIFY THESE VALUES
$replyUrl="https://$tenantId.b2clogin.com/$tenantId.onmicrosoft.com"
$webAppDisplayName = "IdentityExperienceFramework"
$nativeAppDisplayName = "ProxyIdentityExperienceFramework"

$msGraphModule = Get-Module 'Microsoft.Graph' -ListAvailable -ErrorAction SilentlyContinue

if (!$msGraphModule)
{
Write-Host "Microsoft.Graph module is not installed."  -ForegroundColor Yellow
EXIT 1
}

Write-Host "Connecting $TenantId..." -NoNewline;

try
{
 Connect-MgGraph -TenantId "$tenantId.onmicrosoft.com" -Scopes "Application.ReadWrite.All" -ErrorAction Stop >$null
}
catch
{
Write-Host $_.Exception.Message -ForegroundColor Yellow
EXIT 1
}

Write-Host -ForegroundColor Green "Done"

$msGraphServicePrincipalQueryResult=(Get-MgServicePrincipal -Filter "displayName eq 'Microsoft Graph'")
if ($msGraphServicePrincipalQueryResult -is [array]) {
 $aad = $msGraphServicePrincipalQueryResult[0]
} else {
 $aad = $msGraphServicePrincipalQueryResult
}

Write-Host "Reading directory write data role..." -NoNewline;

#  Resource Access User.Read + Sign in & Directory.ReadWrite.All
$graphResourceId = "00000003-0000-0000-c000-000000000000"
$OfflineAccess = @{
  Id="7427e0e9-2fba-42fe-b0c0-848c9e6a8182"
  Type="Scope"
}
$OpenIdAccess = @{
  Id="37f7f235-527c-4136-accd-4a02d197296e"
  Type="Scope"
}
$UserReadAccess = @{
  Id="df021288-bdef-4463-88db-98f22de89214"
  Type="Role"
}
$DirectoryReadWriteAllAccess = @{
  Id="19dbc75e-c2e2-444c-a770-ec69d8559fc7"
  Type="Role"
}

Write-Host -ForegroundColor Green "Done"

# Creating web application
Write-Host "Creating AzureAD Web application : $webAppDisplayName..." -NoNewline;

$oauth2PermissionsScopes = @{
    oauth2PermissionScopes = @(
        @{
            AdminConsentDescription = "Allow the application to access $($webAppDisplayName) on behalf of the signed-in user."    
            AdminConsentDisplayName = "Access $($webAppDisplayName)"
            UserConsentDescription  = "Allow the application to access $($webAppDisplayName) on your behalf."
            UserConsentDisplayName  = "Access $($webAppDisplayName)"
            Id                      = New-Guid
            IsEnabled               = $true
            Type                    = "User"
            Value                   = "user_impersonation"
        }
    )
}

try
{
$app = New-MgApplication -DisplayName $webAppDisplayName -Web @{ RedirectUris= "$replyUrl"; } -RequiredResourceAccess @{ ResourceAppId=$graphResourceId; ResourceAccess=$OfflineAccess, $OpenIdAccess, $UserReadAccess, $DirectoryReadWriteAllAccess } -Api $oauth2PermissionsScopes -ErrorAction Stop
}
catch
{
Write-Host $_.Exception.Message -ForegroundColor Yellow
EXIT 1
}

Write-Host -ForegroundColor Green "Done"

Write-Host "Creating Service prinicipal..." -NoNewline;

$spSvc = New-MgServicePrincipal -AppId $app.AppId

Write-Host -ForegroundColor Green "Done"

Write-Host "Reading directory read data role..." -NoNewline;

#  Grab the User.Read permission
$userRead = $aad.Oauth2PermissionScopes | ? {$_.Value -eq "User.Read"}

#  Grab the user-impersonation permission
$svcUserImpersonation = $spSvc.Oauth2PermissionScopes | ?{$_.Value -eq "user_impersonation"}

#  Resource Access for both Grab permission

$resourceAccess = @(
@{
  resourceAppId=$aad.AppId ;
  resourceAccess=@(@{
    Id = $userRead.Id ;
    Type = "Scope"})
},
@{
  resourceAppId=$app.AppId ;
  resourceAccess=@(@{
    Id = $svcUserImpersonation.Id ;
    Type = "Scope"})
}
)

Write-Host -ForegroundColor Green "Done"
    
#Creating native application
Write-Host "Creating AzureAD Native application ($nativeAppDisplayName)..." -NoNewline;
$nativeApp = New-MgApplication -DisplayName $nativeAppDisplayName -PublicClient @{} -RequiredResourceAccess $resourceAccess
Write-Host -ForegroundColor Green "Done"

# Updating policy files
Function updatePolicyfiles()
{
param(
        [string]$TenantId,
        [string]$Appid,
        [string]$ProxyId,
        [string]$AppObjetId      
    )

$Currentlocation=(Get-Location).path

$configFiles = Get-ChildItem "$CurrentLocation\Policies" *.xml -rec
foreach ($file in $configFiles)
{
    (Get-Content $file.PSPath) |
    Foreach-Object { 
    $_ -replace "B2CTENANT", $TenantId `
    -replace "ProxyIdentityExperienceFrameworkAppId", $ProxyId `
    -replace "IdentityExperienceFrameworkAppId", $Appid `
    -replace "IdentityExperienceFrameworkObjectId", $AppObjetId 
    } |
    Set-Content $file.PSPath
}
}

$appId = $app.AppId
$appObjectId = $app.Id
$proxyAppId = $nativeApp.AppId
$proxyAppObjectId = $nativeApp.Id
$PropertyName = $extensionProperty.Name

Write-Host "Updating B2C policy files..." -NoNewline; 
updatePolicyfiles -TenantId $TenantId -Appid $appId -ProxyId $proxyAppId -AppObjetId $appObjectId
Write-Host -ForegroundColor Green "Done"

$Currentlocation=(Get-Location).path
$outputFile="$Currentlocation\configuration.txt"

If (test-path $outputFile) {Remove-Item $outputFile}
New-Item $outputFile >$null

Write-Host "Web Application Id : " -NoNewline;
Write-Host -ForegroundColor Cyan $appId
Add-Content $outputFile  "Web Application Id : $appId"

Write-Host "Web Application Object Id : " -NoNewline;
Write-Host -ForegroundColor Cyan $appObjectId
Add-Content $outputFile  "Web Application Object Id : $appObjectId"

Write-Host "Service Principal Id : " -NoNewline;
Write-Host -ForegroundColor Cyan $spSvc.AppId

Write-Host "Service Principal Object Id : " -NoNewline;
Write-Host -ForegroundColor Cyan $spSvc.Id

Write-Host "Proxy Application Id : " -NoNewline;
Write-Host -ForegroundColor Cyan $proxyAppId
Add-Content $outputFile  "Native Application Id : $proxyAppId"

Write-Host "Proxy Object Id : " -NoNewline;
Write-Host -ForegroundColor Cyan $proxyAppObjectId
Add-Content $outputFile  "Native Application Object Id: $proxyAppObjectId"

Write-Host "Pre-installation is completed."
# SIG # Begin signature block
# MIIoRgYJKoZIhvcNAQcCoIIoNzCCKDMCAQExDzANBglghkgBZQMEAgEFADB5Bgor
# BgEEAYI3AgEEoGswaTA0BgorBgEEAYI3AgEeMCYCAwEAAAQQH8w7YFlLCE63JNLG
# KX7zUQIBAAIBAAIBAAIBAAIBADAxMA0GCWCGSAFlAwQCAQUABCDzImH2uICy95J/
# 5+uaYJcPOtpTSyFCWIwTqV9cHJRpM6CCDXYwggX0MIID3KADAgECAhMzAAAEBGx0
# Bv9XKydyAAAAAAQEMA0GCSqGSIb3DQEBCwUAMH4xCzAJBgNVBAYTAlVTMRMwEQYD
# VQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNy
# b3NvZnQgQ29ycG9yYXRpb24xKDAmBgNVBAMTH01pY3Jvc29mdCBDb2RlIFNpZ25p
# bmcgUENBIDIwMTEwHhcNMjQwOTEyMjAxMTE0WhcNMjUwOTExMjAxMTE0WjB0MQsw
# CQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9u
# ZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMR4wHAYDVQQDExVNaWNy
# b3NvZnQgQ29ycG9yYXRpb24wggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIB
# AQC0KDfaY50MDqsEGdlIzDHBd6CqIMRQWW9Af1LHDDTuFjfDsvna0nEuDSYJmNyz
# NB10jpbg0lhvkT1AzfX2TLITSXwS8D+mBzGCWMM/wTpciWBV/pbjSazbzoKvRrNo
# DV/u9omOM2Eawyo5JJJdNkM2d8qzkQ0bRuRd4HarmGunSouyb9NY7egWN5E5lUc3
# a2AROzAdHdYpObpCOdeAY2P5XqtJkk79aROpzw16wCjdSn8qMzCBzR7rvH2WVkvF
# HLIxZQET1yhPb6lRmpgBQNnzidHV2Ocxjc8wNiIDzgbDkmlx54QPfw7RwQi8p1fy
# 4byhBrTjv568x8NGv3gwb0RbAgMBAAGjggFzMIIBbzAfBgNVHSUEGDAWBgorBgEE
# AYI3TAgBBggrBgEFBQcDAzAdBgNVHQ4EFgQU8huhNbETDU+ZWllL4DNMPCijEU4w
# RQYDVR0RBD4wPKQ6MDgxHjAcBgNVBAsTFU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjEW
# MBQGA1UEBRMNMjMwMDEyKzUwMjkyMzAfBgNVHSMEGDAWgBRIbmTlUAXTgqoXNzci
# tW2oynUClTBUBgNVHR8ETTBLMEmgR6BFhkNodHRwOi8vd3d3Lm1pY3Jvc29mdC5j
# b20vcGtpb3BzL2NybC9NaWNDb2RTaWdQQ0EyMDExXzIwMTEtMDctMDguY3JsMGEG
# CCsGAQUFBwEBBFUwUzBRBggrBgEFBQcwAoZFaHR0cDovL3d3dy5taWNyb3NvZnQu
# Y29tL3BraW9wcy9jZXJ0cy9NaWNDb2RTaWdQQ0EyMDExXzIwMTEtMDctMDguY3J0
# MAwGA1UdEwEB/wQCMAAwDQYJKoZIhvcNAQELBQADggIBAIjmD9IpQVvfB1QehvpC
# Ge7QeTQkKQ7j3bmDMjwSqFL4ri6ae9IFTdpywn5smmtSIyKYDn3/nHtaEn0X1NBj
# L5oP0BjAy1sqxD+uy35B+V8wv5GrxhMDJP8l2QjLtH/UglSTIhLqyt8bUAqVfyfp
# h4COMRvwwjTvChtCnUXXACuCXYHWalOoc0OU2oGN+mPJIJJxaNQc1sjBsMbGIWv3
# cmgSHkCEmrMv7yaidpePt6V+yPMik+eXw3IfZ5eNOiNgL1rZzgSJfTnvUqiaEQ0X
# dG1HbkDv9fv6CTq6m4Ty3IzLiwGSXYxRIXTxT4TYs5VxHy2uFjFXWVSL0J2ARTYL
# E4Oyl1wXDF1PX4bxg1yDMfKPHcE1Ijic5lx1KdK1SkaEJdto4hd++05J9Bf9TAmi
# u6EK6C9Oe5vRadroJCK26uCUI4zIjL/qG7mswW+qT0CW0gnR9JHkXCWNbo8ccMk1
# sJatmRoSAifbgzaYbUz8+lv+IXy5GFuAmLnNbGjacB3IMGpa+lbFgih57/fIhamq
# 5VhxgaEmn/UjWyr+cPiAFWuTVIpfsOjbEAww75wURNM1Imp9NJKye1O24EspEHmb
# DmqCUcq7NqkOKIG4PVm3hDDED/WQpzJDkvu4FrIbvyTGVU01vKsg4UfcdiZ0fQ+/
# V0hf8yrtq9CkB8iIuk5bBxuPMIIHejCCBWKgAwIBAgIKYQ6Q0gAAAAAAAzANBgkq
# hkiG9w0BAQsFADCBiDELMAkGA1UEBhMCVVMxEzARBgNVBAgTCldhc2hpbmd0b24x
# EDAOBgNVBAcTB1JlZG1vbmQxHjAcBgNVBAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlv
# bjEyMDAGA1UEAxMpTWljcm9zb2Z0IFJvb3QgQ2VydGlmaWNhdGUgQXV0aG9yaXR5
# IDIwMTEwHhcNMTEwNzA4MjA1OTA5WhcNMjYwNzA4MjEwOTA5WjB+MQswCQYDVQQG
# EwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwG
# A1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMSgwJgYDVQQDEx9NaWNyb3NvZnQg
# Q29kZSBTaWduaW5nIFBDQSAyMDExMIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIIC
# CgKCAgEAq/D6chAcLq3YbqqCEE00uvK2WCGfQhsqa+laUKq4BjgaBEm6f8MMHt03
# a8YS2AvwOMKZBrDIOdUBFDFC04kNeWSHfpRgJGyvnkmc6Whe0t+bU7IKLMOv2akr
# rnoJr9eWWcpgGgXpZnboMlImEi/nqwhQz7NEt13YxC4Ddato88tt8zpcoRb0Rrrg
# OGSsbmQ1eKagYw8t00CT+OPeBw3VXHmlSSnnDb6gE3e+lD3v++MrWhAfTVYoonpy
# 4BI6t0le2O3tQ5GD2Xuye4Yb2T6xjF3oiU+EGvKhL1nkkDstrjNYxbc+/jLTswM9
# sbKvkjh+0p2ALPVOVpEhNSXDOW5kf1O6nA+tGSOEy/S6A4aN91/w0FK/jJSHvMAh
# dCVfGCi2zCcoOCWYOUo2z3yxkq4cI6epZuxhH2rhKEmdX4jiJV3TIUs+UsS1Vz8k
# A/DRelsv1SPjcF0PUUZ3s/gA4bysAoJf28AVs70b1FVL5zmhD+kjSbwYuER8ReTB
# w3J64HLnJN+/RpnF78IcV9uDjexNSTCnq47f7Fufr/zdsGbiwZeBe+3W7UvnSSmn
# Eyimp31ngOaKYnhfsi+E11ecXL93KCjx7W3DKI8sj0A3T8HhhUSJxAlMxdSlQy90
# lfdu+HggWCwTXWCVmj5PM4TasIgX3p5O9JawvEagbJjS4NaIjAsCAwEAAaOCAe0w
# ggHpMBAGCSsGAQQBgjcVAQQDAgEAMB0GA1UdDgQWBBRIbmTlUAXTgqoXNzcitW2o
# ynUClTAZBgkrBgEEAYI3FAIEDB4KAFMAdQBiAEMAQTALBgNVHQ8EBAMCAYYwDwYD
# VR0TAQH/BAUwAwEB/zAfBgNVHSMEGDAWgBRyLToCMZBDuRQFTuHqp8cx0SOJNDBa
# BgNVHR8EUzBRME+gTaBLhklodHRwOi8vY3JsLm1pY3Jvc29mdC5jb20vcGtpL2Ny
# bC9wcm9kdWN0cy9NaWNSb29DZXJBdXQyMDExXzIwMTFfMDNfMjIuY3JsMF4GCCsG
# AQUFBwEBBFIwUDBOBggrBgEFBQcwAoZCaHR0cDovL3d3dy5taWNyb3NvZnQuY29t
# L3BraS9jZXJ0cy9NaWNSb29DZXJBdXQyMDExXzIwMTFfMDNfMjIuY3J0MIGfBgNV
# HSAEgZcwgZQwgZEGCSsGAQQBgjcuAzCBgzA/BggrBgEFBQcCARYzaHR0cDovL3d3
# dy5taWNyb3NvZnQuY29tL3BraW9wcy9kb2NzL3ByaW1hcnljcHMuaHRtMEAGCCsG
# AQUFBwICMDQeMiAdAEwAZQBnAGEAbABfAHAAbwBsAGkAYwB5AF8AcwB0AGEAdABl
# AG0AZQBuAHQALiAdMA0GCSqGSIb3DQEBCwUAA4ICAQBn8oalmOBUeRou09h0ZyKb
# C5YR4WOSmUKWfdJ5DJDBZV8uLD74w3LRbYP+vj/oCso7v0epo/Np22O/IjWll11l
# hJB9i0ZQVdgMknzSGksc8zxCi1LQsP1r4z4HLimb5j0bpdS1HXeUOeLpZMlEPXh6
# I/MTfaaQdION9MsmAkYqwooQu6SpBQyb7Wj6aC6VoCo/KmtYSWMfCWluWpiW5IP0
# wI/zRive/DvQvTXvbiWu5a8n7dDd8w6vmSiXmE0OPQvyCInWH8MyGOLwxS3OW560
# STkKxgrCxq2u5bLZ2xWIUUVYODJxJxp/sfQn+N4sOiBpmLJZiWhub6e3dMNABQam
# ASooPoI/E01mC8CzTfXhj38cbxV9Rad25UAqZaPDXVJihsMdYzaXht/a8/jyFqGa
# J+HNpZfQ7l1jQeNbB5yHPgZ3BtEGsXUfFL5hYbXw3MYbBL7fQccOKO7eZS/sl/ah
# XJbYANahRr1Z85elCUtIEJmAH9AAKcWxm6U/RXceNcbSoqKfenoi+kiVH6v7RyOA
# 9Z74v2u3S5fi63V4GuzqN5l5GEv/1rMjaHXmr/r8i+sLgOppO6/8MO0ETI7f33Vt
# Y5E90Z1WTk+/gFcioXgRMiF670EKsT/7qMykXcGhiJtXcVZOSEXAQsmbdlsKgEhr
# /Xmfwb1tbWrJUnMTDXpQzTGCGiYwghoiAgEBMIGVMH4xCzAJBgNVBAYTAlVTMRMw
# EQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVN
# aWNyb3NvZnQgQ29ycG9yYXRpb24xKDAmBgNVBAMTH01pY3Jvc29mdCBDb2RlIFNp
# Z25pbmcgUENBIDIwMTECEzMAAAQEbHQG/1crJ3IAAAAABAQwDQYJYIZIAWUDBAIB
# BQCgga4wGQYJKoZIhvcNAQkDMQwGCisGAQQBgjcCAQQwHAYKKwYBBAGCNwIBCzEO
# MAwGCisGAQQBgjcCARUwLwYJKoZIhvcNAQkEMSIEIHo6Mtjqnj6Undiua8ZBN3cg
# RvJh9Mfc6UJ5nhTOIkPYMEIGCisGAQQBgjcCAQwxNDAyoBSAEgBNAGkAYwByAG8A
# cwBvAGYAdKEagBhodHRwOi8vd3d3Lm1pY3Jvc29mdC5jb20wDQYJKoZIhvcNAQEB
# BQAEggEARA6B9E9JF/TPPHCVGCuZ4EPhA3gjBhNq3UNQbUc/LD2DBquX4aQAL/wo
# cth0pyv93MMRczqxgx13rGQn/1n09jkL95h82LswMV4lmWiZ2h2Kays4fozF+LV1
# wJgLnXtPceOBVMStEXYgDnL22/GGCH4mmrjJiTZodVhLLIoV2P8YBGIJ9QF3qvnZ
# ptUxNGfUMhQaFX3z7BYjqvxeh6A1Ppl1biTtcjjR0NyunerlhO3x84CyVS+kXGlX
# xqA6m/jXsrFWyKTdKubJD3vQ4RMzAdfaV/lJyqM6m/vuHWoUzPDvM0iHa16WET9a
# fnGGYfkbGAXQu7Odyza9bgizJ8w3mqGCF7AwghesBgorBgEEAYI3AwMBMYIXnDCC
# F5gGCSqGSIb3DQEHAqCCF4kwgheFAgEDMQ8wDQYJYIZIAWUDBAIBBQAwggFaBgsq
# hkiG9w0BCRABBKCCAUkEggFFMIIBQQIBAQYKKwYBBAGEWQoDATAxMA0GCWCGSAFl
# AwQCAQUABCBcW3FxhQscii+GP/GyD1zw002JDZHR9oLg8AiYi8SfTQIGZutolh3q
# GBMyMDI0MTAyMzEwMTIzMC43MzhaMASAAgH0oIHZpIHWMIHTMQswCQYDVQQGEwJV
# UzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UE
# ChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMS0wKwYDVQQLEyRNaWNyb3NvZnQgSXJl
# bGFuZCBPcGVyYXRpb25zIExpbWl0ZWQxJzAlBgNVBAsTHm5TaGllbGQgVFNTIEVT
# Tjo1NzFBLTA1RTAtRDk0NzElMCMGA1UEAxMcTWljcm9zb2Z0IFRpbWUtU3RhbXAg
# U2VydmljZaCCEf4wggcoMIIFEKADAgECAhMzAAAB+8vLbDdn5TCVAAEAAAH7MA0G
# CSqGSIb3DQEBCwUAMHwxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9u
# MRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRp
# b24xJjAkBgNVBAMTHU1pY3Jvc29mdCBUaW1lLVN0YW1wIFBDQSAyMDEwMB4XDTI0
# MDcyNTE4MzExM1oXDTI1MTAyMjE4MzExM1owgdMxCzAJBgNVBAYTAlVTMRMwEQYD
# VQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNy
# b3NvZnQgQ29ycG9yYXRpb24xLTArBgNVBAsTJE1pY3Jvc29mdCBJcmVsYW5kIE9w
# ZXJhdGlvbnMgTGltaXRlZDEnMCUGA1UECxMeblNoaWVsZCBUU1MgRVNOOjU3MUEt
# MDVFMC1EOTQ3MSUwIwYDVQQDExxNaWNyb3NvZnQgVGltZS1TdGFtcCBTZXJ2aWNl
# MIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEAqMJWQeWAq4LwvSjYsjP0
# Uvhvm0j0aAOJiMLg0sLfxKoTXAdKD6oMuq5rF5oEiOxV+9ox0H95Q8fhoZq3x9lx
# guZyTOK4l2xtcgtJCtjXRllM2bTpjOg35RUrBy0cAloBU9GJBs7LBNrcbH6rBiOv
# qDQNicPRZwq16xyjMidU1J1AJuat9yLn7taifoD58blYEcBvkj5dH1la9zU846QD
# eOoRO6NcqHLsDx8/zVKZxP30mW6Y7RMsqtB8cGCgGwVVurOnaNLXs31qTRTyVHX8
# ppOdoSihCXeqebgJCRzG8zG/e/k0oaBjFFGl+8uFELwCyh4wK9Z5+azTzfa2GD4p
# 6ihtskXs3lnW05UKfDJhAADt6viOc0Rk/c8zOiqzh0lKpf/eWUY2o/hvcDPZNgLa
# HvyfDqb8AWaKvO36iRZSXqhSw8SxJo0TCpsbCjmtx0LpHnqbb1UF7cq09kCcfWTD
# PcN12pbYLqck0bIIfPKbc7HnrkNQks/mSbVZTnDyT3O8zF9q4DCfWesSr1akycDd
# uGxCdKBvgtJh1YxDq1skTweYx5iAWXnB7KMyls3WQZbTubTCLLt8Xn8t+slcKm5D
# kvobubmHSriuTA3wTyIy4FxamTKm0VDu9mWds8MtjUSJVwNVVlBXaQ3ZMcVjijyV
# oUNVuBY9McwYcIQK62wQ20ECAwEAAaOCAUkwggFFMB0GA1UdDgQWBBRHVSGYUNQ3
# RwOl71zIAuUjIKg1KjAfBgNVHSMEGDAWgBSfpxVdAF5iXYP05dJlpxtTNRnpcjBf
# BgNVHR8EWDBWMFSgUqBQhk5odHRwOi8vd3d3Lm1pY3Jvc29mdC5jb20vcGtpb3Bz
# L2NybC9NaWNyb3NvZnQlMjBUaW1lLVN0YW1wJTIwUENBJTIwMjAxMCgxKS5jcmww
# bAYIKwYBBQUHAQEEYDBeMFwGCCsGAQUFBzAChlBodHRwOi8vd3d3Lm1pY3Jvc29m
# dC5jb20vcGtpb3BzL2NlcnRzL01pY3Jvc29mdCUyMFRpbWUtU3RhbXAlMjBQQ0El
# MjAyMDEwKDEpLmNydDAMBgNVHRMBAf8EAjAAMBYGA1UdJQEB/wQMMAoGCCsGAQUF
# BwMIMA4GA1UdDwEB/wQEAwIHgDANBgkqhkiG9w0BAQsFAAOCAgEAwzoIKOY2dnUj
# fWuMiGoz/ovoc1e86VwWaZNFdgRmOoQuRe4nLdtZONtTHNk3Sj3nkyBszzxSbZEQ
# 0DduyKHHI5P8V87jFttGnlR0wPP22FAebbvAbutkMMVQMFzhVBWiWD0VAnu9x0fj
# ifLKDAVXLwoun5rCFqwbasXFc7H/0DPiC+DBn3tUxefvcxUCys4+DC3s8CYp7WWX
# pZ8Wb/vdBhDliHmB7pWcmsB83uc4/P2GmAI3HMkOEu7fCaSYoQhouWOr07l/KM4T
# ndylIirm8f2WwXQcFEzmUvISM6ludUwGlVNfTTJUq2bTDEd3tlDKtV9AUY3rrnFw
# HTwJryLtT4IFhvgBfND3mL1eeSakKf7xTII4Jyt15SXhHd5oI/XGjSgykgJrWA57
# rGnAC7ru3/ZbFNCMK/Jj6X8X4L6mBOYa2NGKwH4A37YGDrecJ/qXXWUYvfLYqHGf
# 8ThYl12Yg1rwSKpWLolA/B1eqBw4TRcvVY0IvNNi5sm+//HJ9Aw6NJuR/uDR7X7v
# DXicpXMlRNgFMyADb8AFIvQPdHqcRpRorY+YUGlvzeJx/2gNYyezAokbrFhACsJ2
# BfyeLyCEo6AuwEHn511PKE8dK4JvlmLSoHj7VFR3NHDk3zRkx0ExkmF8aOdpvoKh
# uwBCxoZ/JhbzSzrvZ74GVjKKIyt5FA0wggdxMIIFWaADAgECAhMzAAAAFcXna54C
# m0mZAAAAAAAVMA0GCSqGSIb3DQEBCwUAMIGIMQswCQYDVQQGEwJVUzETMBEGA1UE
# CBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9z
# b2Z0IENvcnBvcmF0aW9uMTIwMAYDVQQDEylNaWNyb3NvZnQgUm9vdCBDZXJ0aWZp
# Y2F0ZSBBdXRob3JpdHkgMjAxMDAeFw0yMTA5MzAxODIyMjVaFw0zMDA5MzAxODMy
# MjVaMHwxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQH
# EwdSZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xJjAkBgNV
# BAMTHU1pY3Jvc29mdCBUaW1lLVN0YW1wIFBDQSAyMDEwMIICIjANBgkqhkiG9w0B
# AQEFAAOCAg8AMIICCgKCAgEA5OGmTOe0ciELeaLL1yR5vQ7VgtP97pwHB9KpbE51
# yMo1V/YBf2xK4OK9uT4XYDP/XE/HZveVU3Fa4n5KWv64NmeFRiMMtY0Tz3cywBAY
# 6GB9alKDRLemjkZrBxTzxXb1hlDcwUTIcVxRMTegCjhuje3XD9gmU3w5YQJ6xKr9
# cmmvHaus9ja+NSZk2pg7uhp7M62AW36MEBydUv626GIl3GoPz130/o5Tz9bshVZN
# 7928jaTjkY+yOSxRnOlwaQ3KNi1wjjHINSi947SHJMPgyY9+tVSP3PoFVZhtaDua
# Rr3tpK56KTesy+uDRedGbsoy1cCGMFxPLOJiss254o2I5JasAUq7vnGpF1tnYN74
# kpEeHT39IM9zfUGaRnXNxF803RKJ1v2lIH1+/NmeRd+2ci/bfV+AutuqfjbsNkz2
# K26oElHovwUDo9Fzpk03dJQcNIIP8BDyt0cY7afomXw/TNuvXsLz1dhzPUNOwTM5
# TI4CvEJoLhDqhFFG4tG9ahhaYQFzymeiXtcodgLiMxhy16cg8ML6EgrXY28MyTZk
# i1ugpoMhXV8wdJGUlNi5UPkLiWHzNgY1GIRH29wb0f2y1BzFa/ZcUlFdEtsluq9Q
# BXpsxREdcu+N+VLEhReTwDwV2xo3xwgVGD94q0W29R6HXtqPnhZyacaue7e3Pmri
# Lq0CAwEAAaOCAd0wggHZMBIGCSsGAQQBgjcVAQQFAgMBAAEwIwYJKwYBBAGCNxUC
# BBYEFCqnUv5kxJq+gpE8RjUpzxD/LwTuMB0GA1UdDgQWBBSfpxVdAF5iXYP05dJl
# pxtTNRnpcjBcBgNVHSAEVTBTMFEGDCsGAQQBgjdMg30BATBBMD8GCCsGAQUFBwIB
# FjNodHRwOi8vd3d3Lm1pY3Jvc29mdC5jb20vcGtpb3BzL0RvY3MvUmVwb3NpdG9y
# eS5odG0wEwYDVR0lBAwwCgYIKwYBBQUHAwgwGQYJKwYBBAGCNxQCBAweCgBTAHUA
# YgBDAEEwCwYDVR0PBAQDAgGGMA8GA1UdEwEB/wQFMAMBAf8wHwYDVR0jBBgwFoAU
# 1fZWy4/oolxiaNE9lJBb186aGMQwVgYDVR0fBE8wTTBLoEmgR4ZFaHR0cDovL2Ny
# bC5taWNyb3NvZnQuY29tL3BraS9jcmwvcHJvZHVjdHMvTWljUm9vQ2VyQXV0XzIw
# MTAtMDYtMjMuY3JsMFoGCCsGAQUFBwEBBE4wTDBKBggrBgEFBQcwAoY+aHR0cDov
# L3d3dy5taWNyb3NvZnQuY29tL3BraS9jZXJ0cy9NaWNSb29DZXJBdXRfMjAxMC0w
# Ni0yMy5jcnQwDQYJKoZIhvcNAQELBQADggIBAJ1VffwqreEsH2cBMSRb4Z5yS/yp
# b+pcFLY+TkdkeLEGk5c9MTO1OdfCcTY/2mRsfNB1OW27DzHkwo/7bNGhlBgi7ulm
# ZzpTTd2YurYeeNg2LpypglYAA7AFvonoaeC6Ce5732pvvinLbtg/SHUB2RjebYIM
# 9W0jVOR4U3UkV7ndn/OOPcbzaN9l9qRWqveVtihVJ9AkvUCgvxm2EhIRXT0n4ECW
# OKz3+SmJw7wXsFSFQrP8DJ6LGYnn8AtqgcKBGUIZUnWKNsIdw2FzLixre24/LAl4
# FOmRsqlb30mjdAy87JGA0j3mSj5mO0+7hvoyGtmW9I/2kQH2zsZ0/fZMcm8Qq3Uw
# xTSwethQ/gpY3UA8x1RtnWN0SCyxTkctwRQEcb9k+SS+c23Kjgm9swFXSVRk2XPX
# fx5bRAGOWhmRaw2fpCjcZxkoJLo4S5pu+yFUa2pFEUep8beuyOiJXk+d0tBMdrVX
# VAmxaQFEfnyhYWxz/gq77EFmPWn9y8FBSX5+k77L+DvktxW/tM4+pTFRhLy/AsGC
# onsXHRWJjXD+57XQKBqJC4822rpM+Zv/Cuk0+CQ1ZyvgDbjmjJnW4SLq8CdCPSWU
# 5nR0W2rRnj7tfqAxM328y+l7vzhwRNGQ8cirOoo6CGJ/2XBjU02N7oJtpQUQwXEG
# ahC0HVUzWLOhcGbyoYIDWTCCAkECAQEwggEBoYHZpIHWMIHTMQswCQYDVQQGEwJV
# UzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UE
# ChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMS0wKwYDVQQLEyRNaWNyb3NvZnQgSXJl
# bGFuZCBPcGVyYXRpb25zIExpbWl0ZWQxJzAlBgNVBAsTHm5TaGllbGQgVFNTIEVT
# Tjo1NzFBLTA1RTAtRDk0NzElMCMGA1UEAxMcTWljcm9zb2Z0IFRpbWUtU3RhbXAg
# U2VydmljZaIjCgEBMAcGBSsOAwIaAxUABHHn7NCGusZz2RfVbyuwYwPykBWggYMw
# gYCkfjB8MQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UE
# BxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMSYwJAYD
# VQQDEx1NaWNyb3NvZnQgVGltZS1TdGFtcCBQQ0EgMjAxMDANBgkqhkiG9w0BAQsF
# AAIFAOrCt8swIhgPMjAyNDEwMjIyMzQ2MTlaGA8yMDI0MTAyMzIzNDYxOVowdzA9
# BgorBgEEAYRZCgQBMS8wLTAKAgUA6sK3ywIBADAKAgEAAgIEcwIB/zAHAgEAAgIU
# ujAKAgUA6sQJSwIBADA2BgorBgEEAYRZCgQCMSgwJjAMBgorBgEEAYRZCgMCoAow
# CAIBAAIDB6EgoQowCAIBAAIDAYagMA0GCSqGSIb3DQEBCwUAA4IBAQB1N4koU260
# XmR3nXHXAPAqZu7pTKcYmPK8T9S4Bc6B9P2pCGcnbl0n7zSsjyUEkSyBcmlX5MGO
# XtrKhDcxqPC1BXeR5B2z/xrdGhWl0137n7gkYq5nS9kmeZPb6cQINEJ6lk5J6vBp
# hvDOPEPzwbcs4al0aNTRFQxb5iAPCTNnB0lW0VX9AIuBtVl8wATH4M19vXYeXFvC
# Yqj8kHZy7StjxG9HlRZXIL0OUlQp+CaH3xsLA6cZTEUUxyWqN1tI6XQ2d3/J1T3b
# 6+GR+0J7s7Ayn8NCIa4Mx4VLUTI5X0h6fmf5N7sCiSMxVwTOgk4xBpEkEhGxSvgH
# VTQA05jON0G4MYIEDTCCBAkCAQEwgZMwfDELMAkGA1UEBhMCVVMxEzARBgNVBAgT
# Cldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1vbmQxHjAcBgNVBAoTFU1pY3Jvc29m
# dCBDb3Jwb3JhdGlvbjEmMCQGA1UEAxMdTWljcm9zb2Z0IFRpbWUtU3RhbXAgUENB
# IDIwMTACEzMAAAH7y8tsN2flMJUAAQAAAfswDQYJYIZIAWUDBAIBBQCgggFKMBoG
# CSqGSIb3DQEJAzENBgsqhkiG9w0BCRABBDAvBgkqhkiG9w0BCQQxIgQgq2eVMMzu
# PKTImP3ZMkCiHuAq5tgwWwkuuCqT7OxaiYUwgfoGCyqGSIb3DQEJEAIvMYHqMIHn
# MIHkMIG9BCA52wKr/KCFlVNYiWsCLsB4qhjEYEP3xHqYqDu1SSTlGDCBmDCBgKR+
# MHwxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdS
# ZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xJjAkBgNVBAMT
# HU1pY3Jvc29mdCBUaW1lLVN0YW1wIFBDQSAyMDEwAhMzAAAB+8vLbDdn5TCVAAEA
# AAH7MCIEICz9syW2u9eMsCaxrbu1mipnJ7b7SkUL3SKRL85zvupsMA0GCSqGSIb3
# DQEBCwUABIICAJYexgn0e6YVgEZ6p+xhXmTR7uwOWv4Nnwjm+C90GcFWH8Z3pXsf
# 7j9e21q/xmDq4EG6GzyT8s+kQaFOBfdaU+GVzKVzXqbcJz18uOcHGuIbIBw+WzxH
# Zsd+2BD6B3bJT2G+MP6GAYxHPN6m0WmBsMWRwfUAADXLI4oXlZZsgqLVsVk9t602
# j1oyBrBE9mu3h5a7pl/X+2NF/V8fAxu79DdbnZQxRV4t+yObLIylj+qHKOLJ7yO3
# APuTecN4mkY4gGFJU+ZA+TMuEJBK1gAnGcW+ObbAU0cHgfRfE/SLIDvVrtUHvQr+
# Y9v3xRz+sWwGxxzSDXGq6Er0XvZG/fDI/mNoGfb3rUEBDbdNRNmarSiVvPqIHbcx
# i+LSSdeGIhI0ZZ5zMIY7LwV9B+fa+vfMeZPTAOzHXENK+SD5HB35KXItvVyCimXA
# PU3PGlYnQzDbr5yB8ywzlF8kJwfb1tGNkMPajaqMUrG9DQig+vhsL55ywy7YzjTE
# 8ASfzoBSE+KZ3NwrIu2sr1jJylBUG1+tBhdUuPWL8LBaAolGXc/geKXB+qZq25N1
# HdQhxKIRIBkZSceZrAWmV3dqAfdFsYSEAlbURnhQEX/ZYjqI/VcSlkw38ZYhWK16
# R6yaDNl79JEJ/VCOfO/ZlOWA7UKefWTZz+T8hQuoDUx8n4oOprCQxd98
# SIG # End signature block
