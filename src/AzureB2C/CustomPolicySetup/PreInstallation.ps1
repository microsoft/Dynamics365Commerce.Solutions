  param (
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
# MIIoKQYJKoZIhvcNAQcCoIIoGjCCKBYCAQExDzANBglghkgBZQMEAgEFADB5Bgor
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
# /Xmfwb1tbWrJUnMTDXpQzTGCGgkwghoFAgEBMIGVMH4xCzAJBgNVBAYTAlVTMRMw
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
# fnGGYfkbGAXQu7Odyza9bgizJ8w3mqGCF5MwghePBgorBgEEAYI3AwMBMYIXfzCC
# F3sGCSqGSIb3DQEHAqCCF2wwghdoAgEDMQ8wDQYJYIZIAWUDBAIBBQAwggFRBgsq
# hkiG9w0BCRABBKCCAUAEggE8MIIBOAIBAQYKKwYBBAGEWQoDATAxMA0GCWCGSAFl
# AwQCAQUABCBcW3FxhQscii+GP/GyD1zw002JDZHR9oLg8AiYi8SfTQIGaCZglkTV
# GBIyMDI1MDUyODEwMTkwMi4yNVowBIACAfSggdGkgc4wgcsxCzAJBgNVBAYTAlVT
# MRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQK
# ExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xJTAjBgNVBAsTHE1pY3Jvc29mdCBBbWVy
# aWNhIE9wZXJhdGlvbnMxJzAlBgNVBAsTHm5TaGllbGQgVFNTIEVTTjpGMDAyLTA1
# RTAtRDk0NzElMCMGA1UEAxMcTWljcm9zb2Z0IFRpbWUtU3RhbXAgU2VydmljZaCC
# EeowggcgMIIFCKADAgECAhMzAAACBTx1bIJEh83+AAEAAAIFMA0GCSqGSIb3DQEB
# CwUAMHwxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQH
# EwdSZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xJjAkBgNV
# BAMTHU1pY3Jvc29mdCBUaW1lLVN0YW1wIFBDQSAyMDEwMB4XDTI1MDEzMDE5NDI0
# OVoXDTI2MDQyMjE5NDI0OVowgcsxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNo
# aW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29y
# cG9yYXRpb24xJTAjBgNVBAsTHE1pY3Jvc29mdCBBbWVyaWNhIE9wZXJhdGlvbnMx
# JzAlBgNVBAsTHm5TaGllbGQgVFNTIEVTTjpGMDAyLTA1RTAtRDk0NzElMCMGA1UE
# AxMcTWljcm9zb2Z0IFRpbWUtU3RhbXAgU2VydmljZTCCAiIwDQYJKoZIhvcNAQEB
# BQADggIPADCCAgoCggIBAJKS8t93uAXWvbAZ3LzkIVhcdQLzLATIPgtEu/RZgRd5
# 5nS7li4runZxdXNCZk84dM3xNaobZI+VRv7s+V3MUMMCVHe9TymI6OaG72sdbjcz
# Z1uiv2OX2CW6HPBR3ZJyJUZrt/23ru0zcoUpFIxcW0coXcrAEHtpWj5vWrLmn0Na
# WjY3kUasGocwRWU3LOt4digMsv6bx9Kyoy7+JhSrHMrkXhLshjk16YAHwH4DdCDB
# UiLrgokh94plR6JYoJN/ih1SA/cBCKXGHjw/rPsBPggDR0wS0qFgsWzhb8o0Myxi
# vsqlA8pUJwLkTy4Md0p7C5vLN6eRPHLh9/U3eDzKGjk+L0F+NHRXK2uSakN96nwk
# /BxvgE04hc6jWl90dnwS+dHskVkVCKqkxkWU7kIC5Ngfy6Nzk9QeVowAnw0Rr1MU
# lM5IGsHs9GB6H/o0nbG73LE+H+RU30Eayz3cwLpSOmjF4zjHvRvBvCIrxI0cg7wP
# xyqXtVJ69RhuM3g2iAUXCEEKWGh0T/N4Y+rrLqLEPjPrkgdjfPAVBsFVf/D4v9Uc
# 2f8EwazY8YeeVGM78qTw0ik3iyGQVCoDV8zTx+usNI0Rj1UoO2mxSkAXnVjWhDq0
# mFYzV3ed4JeHpv/o35d4c5ELCdjzcr6kUwyGDyKxdBvopGXrmSDxdgF3gnCRsqhV
# AgMBAAGjggFJMIIBRTAdBgNVHQ4EFgQUdHpauzbtvr7IndsRn2jk10vVsvAwHwYD
# VR0jBBgwFoAUn6cVXQBeYl2D9OXSZacbUzUZ6XIwXwYDVR0fBFgwVjBUoFKgUIZO
# aHR0cDovL3d3dy5taWNyb3NvZnQuY29tL3BraW9wcy9jcmwvTWljcm9zb2Z0JTIw
# VGltZS1TdGFtcCUyMFBDQSUyMDIwMTAoMSkuY3JsMGwGCCsGAQUFBwEBBGAwXjBc
# BggrBgEFBQcwAoZQaHR0cDovL3d3dy5taWNyb3NvZnQuY29tL3BraW9wcy9jZXJ0
# cy9NaWNyb3NvZnQlMjBUaW1lLVN0YW1wJTIwUENBJTIwMjAxMCgxKS5jcnQwDAYD
# VR0TAQH/BAIwADAWBgNVHSUBAf8EDDAKBggrBgEFBQcDCDAOBgNVHQ8BAf8EBAMC
# B4AwDQYJKoZIhvcNAQELBQADggIBAA67GTjtvWLvlXzVPHrGjXTE0ivjNnFhV+QX
# lMWraSd08eDNXIueyzD8cqQRlEKBlWmQoQnpjiO8bm26AyL5aO3uFQxKKmT5GkHm
# AVC+HXGYAQvZ+V6DNNYSyePTCsRKmUjlne+B/Z4ZcCv9FyoaNKmi4dsPYdj1jcXQ
# 6XoVEMJX8cQYpEfOfYzmtCkUZKNpxPgOSpViZ6b8Cs59K9WiOcoQhb3XhTEa26El
# Kv2M6jlGpNfsYipu283vOFaV36LdfF2F01x+VDP1iGgWpB7EF6eghGAA8C3AfrzF
# zOv0swLeX0AsSmey87RGIo9cXiXbb859wV99qmGeX9MPCSIl/E7IAx9QXfEj37eL
# NPVZfYIWzZFo4Crd1yiHInD7FEbQTzCQIeqRXnsFtQETMtfwv2UYnUOjFg2mgNfu
# JDMn1B2TKmLl+/vvcTcKHwD62jF4WnoWJ9BnIJzwhwgGUfJqToxtPphNVA2BD+Hw
# UX5Lk6o/sIQIW8gfYq3Y/QaU670LQF9qyGdOOh2a1kVmym1S+KuT/Yc4suMGIyMP
# AmxAAUDMm7Phm1PKiuu8RHXQaX+ZuZWIeqG80AJ9PM+It+MODK+n2zv9se78JqUq
# Z6IlQsaOH+XACPnfX2mCCwYmpuEngVsVz6hTnN99ub+sqNVnyTE0PeKbXRCnjWfa
# 3+qnI/oRMIIHcTCCBVmgAwIBAgITMwAAABXF52ueAptJmQAAAAAAFTANBgkqhkiG
# 9w0BAQsFADCBiDELMAkGA1UEBhMCVVMxEzARBgNVBAgTCldhc2hpbmd0b24xEDAO
# BgNVBAcTB1JlZG1vbmQxHjAcBgNVBAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjEy
# MDAGA1UEAxMpTWljcm9zb2Z0IFJvb3QgQ2VydGlmaWNhdGUgQXV0aG9yaXR5IDIw
# MTAwHhcNMjEwOTMwMTgyMjI1WhcNMzAwOTMwMTgzMjI1WjB8MQswCQYDVQQGEwJV
# UzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UE
# ChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMSYwJAYDVQQDEx1NaWNyb3NvZnQgVGlt
# ZS1TdGFtcCBQQ0EgMjAxMDCCAiIwDQYJKoZIhvcNAQEBBQADggIPADCCAgoCggIB
# AOThpkzntHIhC3miy9ckeb0O1YLT/e6cBwfSqWxOdcjKNVf2AX9sSuDivbk+F2Az
# /1xPx2b3lVNxWuJ+Slr+uDZnhUYjDLWNE893MsAQGOhgfWpSg0S3po5GawcU88V2
# 9YZQ3MFEyHFcUTE3oAo4bo3t1w/YJlN8OWECesSq/XJprx2rrPY2vjUmZNqYO7oa
# ezOtgFt+jBAcnVL+tuhiJdxqD89d9P6OU8/W7IVWTe/dvI2k45GPsjksUZzpcGkN
# yjYtcI4xyDUoveO0hyTD4MmPfrVUj9z6BVWYbWg7mka97aSueik3rMvrg0XnRm7K
# MtXAhjBcTyziYrLNueKNiOSWrAFKu75xqRdbZ2De+JKRHh09/SDPc31BmkZ1zcRf
# NN0Sidb9pSB9fvzZnkXftnIv231fgLrbqn427DZM9ituqBJR6L8FA6PRc6ZNN3SU
# HDSCD/AQ8rdHGO2n6Jl8P0zbr17C89XYcz1DTsEzOUyOArxCaC4Q6oRRRuLRvWoY
# WmEBc8pnol7XKHYC4jMYctenIPDC+hIK12NvDMk2ZItboKaDIV1fMHSRlJTYuVD5
# C4lh8zYGNRiER9vcG9H9stQcxWv2XFJRXRLbJbqvUAV6bMURHXLvjflSxIUXk8A8
# FdsaN8cIFRg/eKtFtvUeh17aj54WcmnGrnu3tz5q4i6tAgMBAAGjggHdMIIB2TAS
# BgkrBgEEAYI3FQEEBQIDAQABMCMGCSsGAQQBgjcVAgQWBBQqp1L+ZMSavoKRPEY1
# Kc8Q/y8E7jAdBgNVHQ4EFgQUn6cVXQBeYl2D9OXSZacbUzUZ6XIwXAYDVR0gBFUw
# UzBRBgwrBgEEAYI3TIN9AQEwQTA/BggrBgEFBQcCARYzaHR0cDovL3d3dy5taWNy
# b3NvZnQuY29tL3BraW9wcy9Eb2NzL1JlcG9zaXRvcnkuaHRtMBMGA1UdJQQMMAoG
# CCsGAQUFBwMIMBkGCSsGAQQBgjcUAgQMHgoAUwB1AGIAQwBBMAsGA1UdDwQEAwIB
# hjAPBgNVHRMBAf8EBTADAQH/MB8GA1UdIwQYMBaAFNX2VsuP6KJcYmjRPZSQW9fO
# mhjEMFYGA1UdHwRPME0wS6BJoEeGRWh0dHA6Ly9jcmwubWljcm9zb2Z0LmNvbS9w
# a2kvY3JsL3Byb2R1Y3RzL01pY1Jvb0NlckF1dF8yMDEwLTA2LTIzLmNybDBaBggr
# BgEFBQcBAQROMEwwSgYIKwYBBQUHMAKGPmh0dHA6Ly93d3cubWljcm9zb2Z0LmNv
# bS9wa2kvY2VydHMvTWljUm9vQ2VyQXV0XzIwMTAtMDYtMjMuY3J0MA0GCSqGSIb3
# DQEBCwUAA4ICAQCdVX38Kq3hLB9nATEkW+Geckv8qW/qXBS2Pk5HZHixBpOXPTEz
# tTnXwnE2P9pkbHzQdTltuw8x5MKP+2zRoZQYIu7pZmc6U03dmLq2HnjYNi6cqYJW
# AAOwBb6J6Gngugnue99qb74py27YP0h1AdkY3m2CDPVtI1TkeFN1JFe53Z/zjj3G
# 82jfZfakVqr3lbYoVSfQJL1AoL8ZthISEV09J+BAljis9/kpicO8F7BUhUKz/Aye
# ixmJ5/ALaoHCgRlCGVJ1ijbCHcNhcy4sa3tuPywJeBTpkbKpW99Jo3QMvOyRgNI9
# 5ko+ZjtPu4b6MhrZlvSP9pEB9s7GdP32THJvEKt1MMU0sHrYUP4KWN1APMdUbZ1j
# dEgssU5HLcEUBHG/ZPkkvnNtyo4JvbMBV0lUZNlz138eW0QBjloZkWsNn6Qo3GcZ
# KCS6OEuabvshVGtqRRFHqfG3rsjoiV5PndLQTHa1V1QJsWkBRH58oWFsc/4Ku+xB
# Zj1p/cvBQUl+fpO+y/g75LcVv7TOPqUxUYS8vwLBgqJ7Fx0ViY1w/ue10CgaiQuP
# Ntq6TPmb/wrpNPgkNWcr4A245oyZ1uEi6vAnQj0llOZ0dFtq0Z4+7X6gMTN9vMvp
# e784cETRkPHIqzqKOghif9lwY1NNje6CbaUFEMFxBmoQtB1VM1izoXBm8qGCA00w
# ggI1AgEBMIH5oYHRpIHOMIHLMQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGlu
# Z3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBv
# cmF0aW9uMSUwIwYDVQQLExxNaWNyb3NvZnQgQW1lcmljYSBPcGVyYXRpb25zMScw
# JQYDVQQLEx5uU2hpZWxkIFRTUyBFU046RjAwMi0wNUUwLUQ5NDcxJTAjBgNVBAMT
# HE1pY3Jvc29mdCBUaW1lLVN0YW1wIFNlcnZpY2WiIwoBATAHBgUrDgMCGgMVANWw
# f2nW0mf6SvAIy+o6FW9ett60oIGDMIGApH4wfDELMAkGA1UEBhMCVVMxEzARBgNV
# BAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1vbmQxHjAcBgNVBAoTFU1pY3Jv
# c29mdCBDb3Jwb3JhdGlvbjEmMCQGA1UEAxMdTWljcm9zb2Z0IFRpbWUtU3RhbXAg
# UENBIDIwMTAwDQYJKoZIhvcNAQELBQACBQDr4VjfMCIYDzIwMjUwNTI4MDk0MTUx
# WhgPMjAyNTA1MjkwOTQxNTFaMHQwOgYKKwYBBAGEWQoEATEsMCowCgIFAOvhWN8C
# AQAwBwIBAAICO0EwBwIBAAICE84wCgIFAOviql8CAQAwNgYKKwYBBAGEWQoEAjEo
# MCYwDAYKKwYBBAGEWQoDAqAKMAgCAQACAwehIKEKMAgCAQACAwGGoDANBgkqhkiG
# 9w0BAQsFAAOCAQEAToeBwtKNzkPI4Yk7CR+37FQbc9oey9kcsxj+XwAGeA+hJaTg
# KdJI22CuQIMrzlMAw1FHaBNUJm9XtrpwtTqXnVE0cTN2CcqbYjoztCKcOl5rcXfa
# eDJaX5pXzxtabtAXpNfzSDSe8XvVO3bZqfDwVuG3PM7XDUbdSsGtu/zb1PfkY1B9
# eiKkbBzcNXhVwA764lRcytAFDymu1RBxSiNb7y0HKf/dULoEW+5Tr0e+SqNvBS3g
# BjTNZ5CpeM/JCNOWKhhF3SV1H0RAohs6M8JpBs/xvg7F6HBbplBMBLpTTDruwiuD
# 2yrzvVwCChN5RqNe3u8D2EaL2aGqCRY0L5PzvTGCBA0wggQJAgEBMIGTMHwxCzAJ
# BgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25k
# MR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xJjAkBgNVBAMTHU1pY3Jv
# c29mdCBUaW1lLVN0YW1wIFBDQSAyMDEwAhMzAAACBTx1bIJEh83+AAEAAAIFMA0G
# CWCGSAFlAwQCAQUAoIIBSjAaBgkqhkiG9w0BCQMxDQYLKoZIhvcNAQkQAQQwLwYJ
# KoZIhvcNAQkEMSIEIGftY/SmJIDLIVBrPU4KUakPJmcjiSAguZknUBxBJs+9MIH6
# BgsqhkiG9w0BCRACLzGB6jCB5zCB5DCBvQQggA0DPdx5jS6aF1YtHawmmrQ4+q0k
# NMBhGaMdWTARb0gwgZgwgYCkfjB8MQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2Fz
# aGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENv
# cnBvcmF0aW9uMSYwJAYDVQQDEx1NaWNyb3NvZnQgVGltZS1TdGFtcCBQQ0EgMjAx
# MAITMwAAAgU8dWyCRIfN/gABAAACBTAiBCAPk+oQEO4z99xi7voqoYS/So8Tr30s
# pn5hJRh+EeYP6zANBgkqhkiG9w0BAQsFAASCAgCNk/yEW4JumOOrju3BWV+YyGi8
# qV8EgeCkHIFtCJCRiXaHA4XLUZLfjeucO/A2bu2R6vx4YMNQ5R04gXpH2lJTNc5O
# FGZKhXUvnpDh6Bj+tFqFpW5lBszP3n6YLEMGn21xdKTHRprFY9hm4RkP+A8bzx3I
# E0tIncLoJo8nUFNuzPwBT8B09oFkxlJzD1fMKHfyxTJqYwb6iImsIvm02jxQ8Gby
# dY1CiTBqv1S60/nyTSWPSzureEYa/R8oYVS5sZvY3o3a/rTvN+yjbGCfqA0uqnxj
# HZjcESHGgODwOSn5C26WAscImrhZGSoiWZOIdPypnRCaMufAkmr3Puzf918cNH1T
# 3Q9rVfi47iD8lKE+LCu2yZKmvfUB7CuBH+Ai0Fr924Sem8jtX99u5HgDRBd2l+aM
# 84CQn9h+YoL5QL095MpeYsv2BLP5blaNGsTpbqFEEIeglTC5RMeAtQSIr5Kg6zt3
# 2PhV2LGUZHXigYRCNl/PvjifBiZ4L6Ikuo3/HH76pCzqI8v/qoIffM1AqDtia11L
# ghjAlkx+m/ABKK3Gmo+wy65KRkuun4Qe59BPrgSKravXaDutvqiFR05fDMC9Aboy
# bBrkjgwDjkrbYaeCyGtkjjvbPgK++zQsDUhXJPT4Ge+1QLFvCshK/Q13M0qZ3O0y
# YvA2GHho2JMt60xUVA==
# SIG # End signature block
