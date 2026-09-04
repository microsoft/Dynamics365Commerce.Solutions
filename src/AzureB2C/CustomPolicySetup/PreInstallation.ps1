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
# MIInUgYJKoZIhvcNAQcCoIInQzCCJz8CAQExDzANBglghkgBZQMEAgEFADB5Bgor
# BgEEAYI3AgEEoGswaTA0BgorBgEEAYI3AgEeMCYCAwEAAAQQH8w7YFlLCE63JNLG
# KX7zUQIBAAIBAAIBAAIBAAIBADAxMA0GCWCGSAFlAwQCAQUABCDzImH2uICy95J/
# 5+uaYJcPOtpTSyFCWIwTqV9cHJRpM6CCDMkwggYEMIID7KADAgECAhMzAAACHPrN
# xZvoL37EAAAAAAIcMA0GCSqGSIb3DQEBCwUAMFcxCzAJBgNVBAYTAlVTMR4wHAYD
# VQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xKDAmBgNVBAMTH01pY3Jvc29mdCBD
# b2RlIFNpZ25pbmcgUENBIDIwMjQwHhcNMjYwNDE2MTg1OTQxWhcNMjcwNDE1MTg1
# OTQxWjB0MQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UE
# BxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMR4wHAYD
# VQQDExVNaWNyb3NvZnQgQ29ycG9yYXRpb24wggEiMA0GCSqGSIb3DQEBAQUAA4IB
# DwAwggEKAoIBAQDVsZfgOKmM31HPfoWOoNEiw0SlCiIxUMC0I9NMWbucKOw/e9lP
# oAoehQVu6SG65V4EPzrYsnBnFPNoi4/HoOdjhz1qkrEt4I6tEcxXU6oOeY9zGveC
# /3iBeuhLYxM3M/PkcUoebF+Nednm8OkdSPoDu8imViHPQq/8CQUu0WRR4rE+dMRf
# rpVqfmNi2qWCX94T4MsepijGVkwE//tJg0ryAiYdHT34LSnlG/RSBZmQRGWZ5g8j
# qnKjRParSqMft1gvjuUTVgtWNZfgcLFSK5Wa0myrq8OPcgTGGsRgun+tnSS+IxDT
# xVsAPH1OzvPjwomguByhUe/OcvUN0D5Wmp7xAgMBAAGjggGqMIIBpjAOBgNVHQ8B
# Af8EBAMCB4AwHwYDVR0lBBgwFgYKKwYBBAGCN0wIAQYIKwYBBQUHAwMwHQYDVR0O
# BBYEFNoH7a2YDjOSwpkp6DHcmUS7J+0yMFQGA1UdEQRNMEukSTBHMS0wKwYDVQQL
# EyRNaWNyb3NvZnQgSXJlbGFuZCBPcGVyYXRpb25zIExpbWl0ZWQxFjAUBgNVBAUT
# DTIzMDAxMis1MDc1NjkwHwYDVR0jBBgwFoAUf1k/VCHarU/vBeXmo9ctBpQSCDEw
# YAYDVR0fBFkwVzBVoFOgUYZPaHR0cDovL3d3dy5taWNyb3NvZnQuY29tL3BraW9w
# cy9jcmwvTWljcm9zb2Z0JTIwQ29kZSUyMFNpZ25pbmclMjBQQ0ElMjAyMDI0LmNy
# bDBtBggrBgEFBQcBAQRhMF8wXQYIKwYBBQUHMAKGUWh0dHA6Ly93d3cubWljcm9z
# b2Z0LmNvbS9wa2lvcHMvY2VydHMvTWljcm9zb2Z0JTIwQ29kZSUyMFNpZ25pbmcl
# MjBQQ0ElMjAyMDI0LmNydDAMBgNVHRMBAf8EAjAAMA0GCSqGSIb3DQEBCwUAA4IC
# AQAUnEqhaRXe0T3hIJjvdQErEkrA/7bByjn6t5IArODkkRjzkYwtKMc2yYj2quaN
# rLutWw2YZcngKPy1b71YyDJQTy4NDRwaSh9Tw5thrk3NmcPrAHia5vtcBJ1CgtKK
# 7mQbIcQ22d/N3813ayCDDFewu1+jsZmX+r/aTEqaOM4TVxVtRSkuCy8nAXKuChOK
# Li/zA4XuH8iEYqIsj2YoNaeSxVmeGiERXpKdo3dDmYi0kO5w2D8VS4c3+9h6gElY
# BaAAg/dYErBg27qT3vv0zRDJhJufvCNylA8S7/+8H5E/PV5cng6na9VV/w9OV3qu
# uND6zdGa2EX38Glp50F9AIQk3p2xXmcvorDeM4XJ7UlWYBi6g80J1SSOQnInCYFE
# msfUNn3+1AaTJKSJL83quKArTac2pKhu0Yzzzrzo6HrsRiQKzpnRBb1/dMa6P3hz
# 75XbMRBctNsFhZC07WCmjExdLg2eHW5uV0TY8D5+6wozJf7vF3+WHkYPO85Z+BC6
# U4FkNbYNycZ9cE4j1tXRdyDCfml6c0HWPHjNVDObrv9lKt3qUqFpX38VCqVCyNOO
# 1UcXfQiVjJw32U2WUKZjt/neJKHEBsm9kFsLuWzkQ53+qcaSaytmsCnk2gOglrlD
# 5d3kKyvvAw+rzm0lT8K38P6PLxfZQHhu4W8dV7Av8N2ZmDCCBr0wggSloAMCAQIC
# EzMAAAA5O7Y3Gb8GHWcAAAAAADkwDQYJKoZIhvcNAQEMBQAwgYgxCzAJBgNVBAYT
# AlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYD
# VQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xMjAwBgNVBAMTKU1pY3Jvc29mdCBS
# b290IENlcnRpZmljYXRlIEF1dGhvcml0eSAyMDExMB4XDTI0MDgwODIwNTQxOFoX
# DTM2MDMyMjIyMTMwNFowVzELMAkGA1UEBhMCVVMxHjAcBgNVBAoTFU1pY3Jvc29m
# dCBDb3Jwb3JhdGlvbjEoMCYGA1UEAxMfTWljcm9zb2Z0IENvZGUgU2lnbmluZyBQ
# Q0EgMjAyNDCCAiIwDQYJKoZIhvcNAQEBBQADggIPADCCAgoCggIBANgBnB7jOMeq
# lRYHNa265v4IY9fH8TKhemHfPINe1gpLaV3dhg324WwH06LcHbpnsBukCDNitryo
# 0dtS/EW6I/yEL/bLSY8hKpbfQuWusBPr9qazYcDxCW/qnjb5JsI1s8bNOg3bVATv
# QVL4tcf03aTycsz8QeCdM0l/yHRObJ9QqazM1r6VPEOJ7LL+uEEb73w6QCuhs89a
# 1uv1zerOYMnsneRRwCbpyW11IcggU0cRKDDq1pjVJzIbIF6+oiXXbReOsgeI8zu1
# FyQfK0fVkaya8SmVHQ/tOf23mZ4W9k0Ri22QW9p3UgSC5OUDktKxxcCmGL6tXLfO
# GSWHIIV4YrTJTT6PNty5REojHJuZHArkF9VnHTERWoTjAzfI3kP+5b4alUdhgAZ7
# ttOu1bVnXfHaqPYl2rPs20ji03LOVWsh/radgE17es5hL+t6lV0eVHrVhsssROWJ
# uz2MXMCt7iw7lFPG9LXKGjsmonn2gotGdHIuEg5JnJMJVmixd5LRlkmgYRZKzhxS
# CwyoGIq0PhaA7Y+VPct5pCHkijcIIDm0nlkK+0KyepolcqGm0T/GYQRMhHJlGOOm
# VQop36wUVUYklUy++vDWeEgEo4s7hxN6mIbf2MSIQ/iIfMZgJxC69oukMUXCrOC3
# SkE/xIkgpfl22MM1itkZ35nNXkMolU1lAgMBAAGjggFOMIIBSjAOBgNVHQ8BAf8E
# BAMCAYYwEAYJKwYBBAGCNxUBBAMCAQAwHQYDVR0OBBYEFH9ZP1Qh2q1P7wXl5qPX
# LQaUEggxMBkGCSsGAQQBgjcUAgQMHgoAUwB1AGIAQwBBMA8GA1UdEwEB/wQFMAMB
# Af8wHwYDVR0jBBgwFoAUci06AjGQQ7kUBU7h6qfHMdEjiTQwWgYDVR0fBFMwUTBP
# oE2gS4ZJaHR0cDovL2NybC5taWNyb3NvZnQuY29tL3BraS9jcmwvcHJvZHVjdHMv
# TWljUm9vQ2VyQXV0MjAxMV8yMDExXzAzXzIyLmNybDBeBggrBgEFBQcBAQRSMFAw
# TgYIKwYBBQUHMAKGQmh0dHA6Ly93d3cubWljcm9zb2Z0LmNvbS9wa2kvY2VydHMv
# TWljUm9vQ2VyQXV0MjAxMV8yMDExXzAzXzIyLmNydDANBgkqhkiG9w0BAQwFAAOC
# AgEAFJQfOChP7onn6fLIMKrSlN1WYKwDFgAddymOUO3FrM8d7B/W/iQ6DxXsDn7D
# 5W4wMwYeLystcEqfkjz4NURRgazyMu5yRzQh4LqjA4tStTcJh1opExo7nn5PuPBY
# nbu0+THSuVHTe0VTTPVhily/piFrDo3axQ9P4C+Ol5yet+2gTfekICS5xS+cYfSI
# vgn0JksVBVMYVI5QFu/qhnLhsEFEUzG8fvv0hjgkO+lkpV9ty6GkN4vdnd7ya6Q6
# aR9y34aiM1qmxaxBi6OUnyNl6fkuun/diTFnYDLTppOkr/mg5WSfCiDVMNCxtj4w
# PKC5OmHm1DQIt/MNokbbH3UGsFP1QbzsLocuSqLCvH09Io3fDPTmscR9Y75G4qX7
# RTX8AdBPo0I6OEojf39zuFZt0qOHm65YWQE69cZM2ueE1MB05dNNgHK9gTE7zKvK
# /fg8B2qjW88MT/WF5V5uvZGtqa9FSL2RazArA+rDPuf6JGYz4HpgMZHB4S6szWSK
# YBv0VisCzfxgeU+dquXW9bd0auYlOB58DPcOYKdc3Se94g+xL4pcEhbB54JOgAkw
# YTu/9dLeH2pDqeJZAABVDWRQCaXfO5LgyKwKCLYXpigrZYCjUSBcr+Ve8PFWMhVT
# Ql0v4q8J/AUmQN5W4n101cY2L4A7GTQG1h32HHAvfQESWP0xghnfMIIZ2wIBATBu
# MFcxCzAJBgNVBAYTAlVTMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24x
# KDAmBgNVBAMTH01pY3Jvc29mdCBDb2RlIFNpZ25pbmcgUENBIDIwMjQCEzMAAAIc
# +s3Fm+gvfsQAAAAAAhwwDQYJYIZIAWUDBAIBBQCggZAwGQYJKoZIhvcNAQkDMQwG
# CisGAQQBgjcCAQQwLwYJKoZIhvcNAQkEMSIEIHo6Mtjqnj6Undiua8ZBN3cgRvJh
# 9Mfc6UJ5nhTOIkPYMEIGCisGAQQBgjcCAQwxNDAyoBSAEgBNAGkAYwByAG8AcwBv
# AGYAdKEagBhodHRwOi8vd3d3Lm1pY3Jvc29mdC5jb20wDQYJKoZIhvcNAQEBBQAE
# ggEAdH4lBQWgdD0Qy09dYKnas8Il6juC/WzMcGCQdIPELbY8Vyh/+TRjKLGt61sM
# R+gQzHMXvpGdI0yVPCNzNgo/OnLsoluSpHrM0N7q1Bjm+QZtQ5pUFZVdbwO8ZtSD
# eh2/vPPmWmfDWj2LHDymoDH+ZkWLkH8l07qKFXgNnFIzrtkraY1CtS7lXVRHycJk
# szUf4gEkOeWUi8zlEn0BvDcApjeAvIOUaHpd9TNMnjQ4K0SdV2EgCr/7u9/b7JZS
# 7v9IN0Gbis+RUYdBeM6KZiE3fiJNPd7TDs/c8hB5ggjx2kGjcjgeGwmgPg5WhHTR
# B5/j1t5agdf9fioCSA1sxYti2KGCF68wgherBgorBgEEAYI3AwMBMYIXmzCCF5cG
# CSqGSIb3DQEHAqCCF4gwgheEAgEDMQ8wDQYJYIZIAWUDBAIBBQAwggFZBgsqhkiG
# 9w0BCRABBKCCAUgEggFEMIIBQAIBAQYKKwYBBAGEWQoDATAxMA0GCWCGSAFlAwQC
# AQUABCAmvHxYquoKqabzvJaZ2Uossp1ysHsiFA94BbM4GT0XvgIGaolRMsENGBIy
# MDI2MDkwNDEwMTQwMS45OFowBIACAfSggdmkgdYwgdMxCzAJBgNVBAYTAlVTMRMw
# EQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVN
# aWNyb3NvZnQgQ29ycG9yYXRpb24xLTArBgNVBAsTJE1pY3Jvc29mdCBJcmVsYW5k
# IE9wZXJhdGlvbnMgTGltaXRlZDEnMCUGA1UECxMeblNoaWVsZCBUU1MgRVNOOjUy
# MUEtMDVFMC1EOTQ3MSUwIwYDVQQDExxNaWNyb3NvZnQgVGltZS1TdGFtcCBTZXJ2
# aWNloIIR/jCCBygwggUQoAMCAQICEzMAAAIXcfsupa8BHeoAAQAAAhcwDQYJKoZI
# hvcNAQELBQAwfDELMAkGA1UEBhMCVVMxEzARBgNVBAgTCldhc2hpbmd0b24xEDAO
# BgNVBAcTB1JlZG1vbmQxHjAcBgNVBAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjEm
# MCQGA1UEAxMdTWljcm9zb2Z0IFRpbWUtU3RhbXAgUENBIDIwMTAwHhcNMjUwODE0
# MTg0ODIzWhcNMjYxMTEzMTg0ODIzWjCB0zELMAkGA1UEBhMCVVMxEzARBgNVBAgT
# Cldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1vbmQxHjAcBgNVBAoTFU1pY3Jvc29m
# dCBDb3Jwb3JhdGlvbjEtMCsGA1UECxMkTWljcm9zb2Z0IElyZWxhbmQgT3BlcmF0
# aW9ucyBMaW1pdGVkMScwJQYDVQQLEx5uU2hpZWxkIFRTUyBFU046NTIxQS0wNUUw
# LUQ5NDcxJTAjBgNVBAMTHE1pY3Jvc29mdCBUaW1lLVN0YW1wIFNlcnZpY2UwggIi
# MA0GCSqGSIb3DQEBAQUAA4ICDwAwggIKAoICAQDAzzawTD7f29hHuIYIgUOdg2FE
# z4HMXYBiKrl1SlmkkU9GMJyKlDpFRsj5EBg1ECkTHHnKCtdtpa27C+qyoBVJZtT9
# bp95y6OMguQ+qf2meC1ZGR9CBtiC9pcAuXo9jbI4f1/vYwP7oDLFB6dKkAx1TNj9
# CT+O2Owd0VdUF762yGUiRIncZnDxVUqpODcZkSXO3BslY22uEES+F/pqfEx6BAwk
# /u07z8EnshOzj8BXcPZu/x4eTzVj586ZDyZDrVYurbAi2+XMhPlvN6Ur0u8TJx/n
# HWVfEDATDlrt2lL4IpNqeG2/5DabB5sDJuN/2KjiTjyy1NqrWu1ys6IPB12pWbVL
# 17yHVOzNcXOMsu8T0SaOc18h7Cxj5EKFRoedjgUXDAkScS/8lLvqgdPSgZ3Lv4nr
# R8Y6XsDVShTICSGlvYGNr5LBIiIdtDgUPImIuVnp2tlnzgygnznYlLEzSEGMY7oV
# mU87yK43KTrzQOLxWdtI2kh0k34OGV6l5NQwEMeoKZ3SZVZFZk+bWm+C3L3dqw23
# 82DqjibZ2eihY50zfIqwlpaPIeqJz2B9OtkEz48Jep5No7WgSJpD6HjDScdV1X5d
# K8jabXI3iKJuqObkc9oA7c4n46Y0t+01WavhnpTZPqkhwsHKygpwNS8KrJxePgL/
# 6fUoUGkMZ0MzD2Ca7wIDAQABo4IBSTCCAUUwHQYDVR0OBBYEFFCs32OXA06h4Tll
# CqcXnHxLVslDMB8GA1UdIwQYMBaAFJ+nFV0AXmJdg/Tl0mWnG1M1GelyMF8GA1Ud
# HwRYMFYwVKBSoFCGTmh0dHA6Ly93d3cubWljcm9zb2Z0LmNvbS9wa2lvcHMvY3Js
# L01pY3Jvc29mdCUyMFRpbWUtU3RhbXAlMjBQQ0ElMjAyMDEwKDEpLmNybDBsBggr
# BgEFBQcBAQRgMF4wXAYIKwYBBQUHMAKGUGh0dHA6Ly93d3cubWljcm9zb2Z0LmNv
# bS9wa2lvcHMvY2VydHMvTWljcm9zb2Z0JTIwVGltZS1TdGFtcCUyMFBDQSUyMDIw
# MTAoMSkuY3J0MAwGA1UdEwEB/wQCMAAwFgYDVR0lAQH/BAwwCgYIKwYBBQUHAwgw
# DgYDVR0PAQH/BAQDAgeAMA0GCSqGSIb3DQEBCwUAA4ICAQBGaAV2EHvEgAgcQSDk
# j/lL6DHrtpHpGJbxkDC0TebPyjR3Kf6kg/6WJ01HUgpBDSv5GNiAj1xnqZu8DK+H
# d7ar8FXyuMcTe530/JjKrfZ64WB1ne9fhvlBd49aWkEBit3OJHbusfPpbCkfl1mx
# LKltcMFtCRaQ2XqDzbJPLcBsFsoNcF3PFmwRq5o6mVq0rsSvh2GCUoF7HwlDZ0xK
# RJnB4I0Nep32v/1bZV1FmwMko/9dzhTJCVWxugGi0q1gRJcWSPBHUdWwf5DQLr38
# 3kI/9OrdiAjW5vhv37cwkUpaElcJwkYVmRwvBSZjCgWDVcujsMsl0aOsgfWOwjY0
# VckVAd5/oB1F7URG9hB6q3KsG/Ei9H4//zcU1jLPHTiKdRP1MXYDBM66oILpnrug
# 3BHikQQnIAgRES+R2GON9ZyCyT+cZOV9qG81j9I4es1Eqjj0oOVxw3NEIlDJD4Pn
# 2vv+p5s1LJA9N/Aj376MRjRD9RYpU0uGKjnkZgGx4n32zs3OR3E6v1R+2nn3scSi
# 9sDr2oaVnc6lQTcfVEEYNWn8W8za5dO6bwusyQ9fHkCn+Rs8BKwL2O72gwJ7YgRc
# 7ZJ4PVoPciq8A50cUoeDW+ls9RBJZvqDbF8FXfTAVypo9iDNxvE9Y/Jmor5LyXvP
# Drho7mGYnt5DCgx9O7RVrLqvjzCCB3EwggVZoAMCAQICEzMAAAAVxedrngKbSZkA
# AAAAABUwDQYJKoZIhvcNAQELBQAwgYgxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpX
# YXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQg
# Q29ycG9yYXRpb24xMjAwBgNVBAMTKU1pY3Jvc29mdCBSb290IENlcnRpZmljYXRl
# IEF1dGhvcml0eSAyMDEwMB4XDTIxMDkzMDE4MjIyNVoXDTMwMDkzMDE4MzIyNVow
# fDELMAkGA1UEBhMCVVMxEzARBgNVBAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1Jl
# ZG1vbmQxHjAcBgNVBAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjEmMCQGA1UEAxMd
# TWljcm9zb2Z0IFRpbWUtU3RhbXAgUENBIDIwMTAwggIiMA0GCSqGSIb3DQEBAQUA
# A4ICDwAwggIKAoICAQDk4aZM57RyIQt5osvXJHm9DtWC0/3unAcH0qlsTnXIyjVX
# 9gF/bErg4r25PhdgM/9cT8dm95VTcVrifkpa/rg2Z4VGIwy1jRPPdzLAEBjoYH1q
# UoNEt6aORmsHFPPFdvWGUNzBRMhxXFExN6AKOG6N7dcP2CZTfDlhAnrEqv1yaa8d
# q6z2Nr41JmTamDu6GnszrYBbfowQHJ1S/rboYiXcag/PXfT+jlPP1uyFVk3v3byN
# pOORj7I5LFGc6XBpDco2LXCOMcg1KL3jtIckw+DJj361VI/c+gVVmG1oO5pGve2k
# rnopN6zL64NF50ZuyjLVwIYwXE8s4mKyzbnijYjklqwBSru+cakXW2dg3viSkR4d
# Pf0gz3N9QZpGdc3EXzTdEonW/aUgfX782Z5F37ZyL9t9X4C626p+Nuw2TPYrbqgS
# Uei/BQOj0XOmTTd0lBw0gg/wEPK3Rxjtp+iZfD9M269ewvPV2HM9Q07BMzlMjgK8
# QmguEOqEUUbi0b1qGFphAXPKZ6Je1yh2AuIzGHLXpyDwwvoSCtdjbwzJNmSLW6Cm
# gyFdXzB0kZSU2LlQ+QuJYfM2BjUYhEfb3BvR/bLUHMVr9lxSUV0S2yW6r1AFemzF
# ER1y7435UsSFF5PAPBXbGjfHCBUYP3irRbb1Hode2o+eFnJpxq57t7c+auIurQID
# AQABo4IB3TCCAdkwEgYJKwYBBAGCNxUBBAUCAwEAATAjBgkrBgEEAYI3FQIEFgQU
# KqdS/mTEmr6CkTxGNSnPEP8vBO4wHQYDVR0OBBYEFJ+nFV0AXmJdg/Tl0mWnG1M1
# GelyMFwGA1UdIARVMFMwUQYMKwYBBAGCN0yDfQEBMEEwPwYIKwYBBQUHAgEWM2h0
# dHA6Ly93d3cubWljcm9zb2Z0LmNvbS9wa2lvcHMvRG9jcy9SZXBvc2l0b3J5Lmh0
# bTATBgNVHSUEDDAKBggrBgEFBQcDCDAZBgkrBgEEAYI3FAIEDB4KAFMAdQBiAEMA
# QTALBgNVHQ8EBAMCAYYwDwYDVR0TAQH/BAUwAwEB/zAfBgNVHSMEGDAWgBTV9lbL
# j+iiXGJo0T2UkFvXzpoYxDBWBgNVHR8ETzBNMEugSaBHhkVodHRwOi8vY3JsLm1p
# Y3Jvc29mdC5jb20vcGtpL2NybC9wcm9kdWN0cy9NaWNSb29DZXJBdXRfMjAxMC0w
# Ni0yMy5jcmwwWgYIKwYBBQUHAQEETjBMMEoGCCsGAQUFBzAChj5odHRwOi8vd3d3
# Lm1pY3Jvc29mdC5jb20vcGtpL2NlcnRzL01pY1Jvb0NlckF1dF8yMDEwLTA2LTIz
# LmNydDANBgkqhkiG9w0BAQsFAAOCAgEAnVV9/Cqt4SwfZwExJFvhnnJL/Klv6lwU
# tj5OR2R4sQaTlz0xM7U518JxNj/aZGx80HU5bbsPMeTCj/ts0aGUGCLu6WZnOlNN
# 3Zi6th542DYunKmCVgADsAW+iehp4LoJ7nvfam++Kctu2D9IdQHZGN5tggz1bSNU
# 5HhTdSRXud2f8449xvNo32X2pFaq95W2KFUn0CS9QKC/GbYSEhFdPSfgQJY4rPf5
# KYnDvBewVIVCs/wMnosZiefwC2qBwoEZQhlSdYo2wh3DYXMuLGt7bj8sCXgU6ZGy
# qVvfSaN0DLzskYDSPeZKPmY7T7uG+jIa2Zb0j/aRAfbOxnT99kxybxCrdTDFNLB6
# 2FD+CljdQDzHVG2dY3RILLFORy3BFARxv2T5JL5zbcqOCb2zAVdJVGTZc9d/HltE
# AY5aGZFrDZ+kKNxnGSgkujhLmm77IVRrakURR6nxt67I6IleT53S0Ex2tVdUCbFp
# AUR+fKFhbHP+CrvsQWY9af3LwUFJfn6Tvsv4O+S3Fb+0zj6lMVGEvL8CwYKiexcd
# FYmNcP7ntdAoGokLjzbaukz5m/8K6TT4JDVnK+ANuOaMmdbhIurwJ0I9JZTmdHRb
# atGePu1+oDEzfbzL6Xu/OHBE0ZDxyKs6ijoIYn/ZcGNTTY3ugm2lBRDBcQZqELQd
# VTNYs6FwZvKhggNZMIICQQIBATCCAQGhgdmkgdYwgdMxCzAJBgNVBAYTAlVTMRMw
# EQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVN
# aWNyb3NvZnQgQ29ycG9yYXRpb24xLTArBgNVBAsTJE1pY3Jvc29mdCBJcmVsYW5k
# IE9wZXJhdGlvbnMgTGltaXRlZDEnMCUGA1UECxMeblNoaWVsZCBUU1MgRVNOOjUy
# MUEtMDVFMC1EOTQ3MSUwIwYDVQQDExxNaWNyb3NvZnQgVGltZS1TdGFtcCBTZXJ2
# aWNloiMKAQEwBwYFKw4DAhoDFQBpsoAVoq3aFpR2qQd8VjMDN+BIy6CBgzCBgKR+
# MHwxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdS
# ZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xJjAkBgNVBAMT
# HU1pY3Jvc29mdCBUaW1lLVN0YW1wIFBDQSAyMDEwMA0GCSqGSIb3DQEBCwUAAgUA
# 7kTxyzAiGA8yMDI2MDkwNDA3MjkxNVoYDzIwMjYwOTA1MDcyOTE1WjB3MD0GCisG
# AQQBhFkKBAExLzAtMAoCBQDuRPHLAgEAMAoCAQACAhLKAgH/MAcCAQACAhOBMAoC
# BQDuRkNLAgEAMDYGCisGAQQBhFkKBAIxKDAmMAwGCisGAQQBhFkKAwKgCjAIAgEA
# AgMHoSChCjAIAgEAAgMBhqAwDQYJKoZIhvcNAQELBQADggEBAJJQ0zddd9JRaOxL
# 3JxATEjyG+H/ughTSixz0YGZbvznP4tb/MuxgLYzVEaXOSbQTmlMVVuYb+dH0DjY
# Byq4PLfGha8gZ0PBXXU/ft3tno/VWqGYyHYVi+sUq/Re5dAUmhtgafVHdbM7NWmD
# bGSfxAcyfrDyYr5adssjN4eVKEs9zm6EmeQYHGGWSQZTI/1fzCubp7PoeNWFOrT+
# q9IK0dpZdzgEnP1hT5ppLntrhMWtFo+c9P3tJ81Kp2P2qZijWxVsjoKfFbooKQx+
# WWt8V1l1FrmwrKVW//r/CHOJoXeJOhgNYrYF+hu7TZZxaUiG+lL1nofiEhdVdUO/
# HJ47eL8xggQNMIIECQIBATCBkzB8MQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2Fz
# aGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENv
# cnBvcmF0aW9uMSYwJAYDVQQDEx1NaWNyb3NvZnQgVGltZS1TdGFtcCBQQ0EgMjAx
# MAITMwAAAhdx+y6lrwEd6gABAAACFzANBglghkgBZQMEAgEFAKCCAUowGgYJKoZI
# hvcNAQkDMQ0GCyqGSIb3DQEJEAEEMC8GCSqGSIb3DQEJBDEiBCB4u8hzZ8JzOXWz
# kVmNULhei/dZI8jZaaDhugc0mcduaDCB+gYLKoZIhvcNAQkQAi8xgeowgecwgeQw
# gb0EINDyUGA+XbfJnRLNRK3mmE4h6Ac/LCuQ3B6/F7aT5FpbMIGYMIGApH4wfDEL
# MAkGA1UEBhMCVVMxEzARBgNVBAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1v
# bmQxHjAcBgNVBAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjEmMCQGA1UEAxMdTWlj
# cm9zb2Z0IFRpbWUtU3RhbXAgUENBIDIwMTACEzMAAAIXcfsupa8BHeoAAQAAAhcw
# IgQgB6bkp9U4tina+24Yo/9WbP47TRJiPHkgLuflp8YtjoowDQYJKoZIhvcNAQEL
# BQAEggIAjkNPKaQx61e4Oi52mvDrOAWXsSLS5W7MBbb94K2Mo2DMJS9uO7V5wjYV
# 7TC3j2nyKt80Ru5ufFZpcehaVAUEHC8MtiiJOotjqegMmpkrPWYYaGIRoAzCB872
# C/uLH5UjtDuBIlow/eIIsMW1lWVL6DNi/o0Y0tk7LsQGvvruwicAUb8x/bc7hwzm
# clVvKN/7K5va8JJK6Ea9ptZM3adfHAZcFhtYz2YY90DJKd96/tCrg04lQQR4ky9i
# IIbVESbve8w6RwQKoPgARC0QJ3tRLrywk92DxSTT5yGAeHCh7iWVsX5qBg8MGZRh
# ebUMthx+iC+Ef9mTW3ifWforM+AIfYLKyuinZ58B4tIx14PRa1zh74vqdyVBZ789
# cpKn423G9vx0kRunXVsd9OI7bDb2pxQ3Q362psjHTK09yC1AGprfb+4CMpCpYylS
# o0nkcbTfY9kdeCmqzaJoa7Cj/fKYlx+kMSZpiDS3uMZ3qvWMbVnMJNOnNIhSA21u
# qXDKKSd0y42ZRHgCWplJDvq+C47J8p1fLd4t9IKx0m/rXvv8HQfKzFRlZg82gNCm
# Vbq1v9j+ZfLCccQHin3488JkSod9kyE6SPv8EGW76UOljNatDpJpam8b23UieOX6
# I4sYTQaT8aOabEnTK9Rea75Hqd57Vjhu17iTZeFwM4Npv4+m78o=
# SIG # End signature block
