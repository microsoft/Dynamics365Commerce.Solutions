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
# MIInbQYJKoZIhvcNAQcCoIInXjCCJ1oCAQExDzANBglghkgBZQMEAgEFADB5Bgor
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
# Ql0v4q8J/AUmQN5W4n101cY2L4A7GTQG1h32HHAvfQESWP0xghn6MIIZ9gIBATBu
# MFcxCzAJBgNVBAYTAlVTMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24x
# KDAmBgNVBAMTH01pY3Jvc29mdCBDb2RlIFNpZ25pbmcgUENBIDIwMjQCEzMAAAIc
# +s3Fm+gvfsQAAAAAAhwwDQYJYIZIAWUDBAIBBQCgga4wGQYJKoZIhvcNAQkDMQwG
# CisGAQQBgjcCAQQwHAYKKwYBBAGCNwIBCzEOMAwGCisGAQQBgjcCARUwLwYJKoZI
# hvcNAQkEMSIEIHo6Mtjqnj6Undiua8ZBN3cgRvJh9Mfc6UJ5nhTOIkPYMEIGCisG
# AQQBgjcCAQwxNDAyoBSAEgBNAGkAYwByAG8AcwBvAGYAdKEagBhodHRwOi8vd3d3
# Lm1pY3Jvc29mdC5jb20wDQYJKoZIhvcNAQEBBQAEggEAEULY3VeyrWZ5A5E+RdTr
# bINpzsh9Alg8QTTdg4HpJnB+F2vKgBqaht/LCG8u2Thou2gEvKHiwZymnGsnuXlf
# JkGT7FhGbTc4uk38hYzlgR7IMysv+kX3270lcXnc1CSyGOZYfzQeD69J1UhXoFIw
# nE9Krhj69dwDroiekOnM856X9fZSy5G4n4ri9o+F0xqIqKQ37DgpkKvgP7QM9QAP
# olMECtU0EBnKzkfumJoiBkxAk3DZeCnauH1ZhFNciNQxANEitorUzNDs4TkhgiXC
# A87IRAutsXrRP8ptCyJ8wWnR2QgmHWrxXbOgDBd9P++uhYhSWc3rpepRcR/LT3ep
# MKGCF6wwgheoBgorBgEEAYI3AwMBMYIXmDCCF5QGCSqGSIb3DQEHAqCCF4UwgheB
# AgEDMQ8wDQYJYIZIAWUDBAIBBQAwggFZBgsqhkiG9w0BCRABBKCCAUgEggFEMIIB
# QAIBAQYKKwYBBAGEWQoDATAxMA0GCWCGSAFlAwQCAQUABCBE2+61zL+3FgEEVsC0
# xR+07Yy2MV0A+tYSqfoN7YcPsAIGaq1vv1IMGBIyMDI2MTAyMTEwMTc1Ni40OFow
# BIACAfSggdmkgdYwgdMxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9u
# MRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRp
# b24xLTArBgNVBAsTJE1pY3Jvc29mdCBJcmVsYW5kIE9wZXJhdGlvbnMgTGltaXRl
# ZDEnMCUGA1UECxMeblNoaWVsZCBUU1MgRVNOOjM2MDUtMDVFMC1EOTQ3MSUwIwYD
# VQQDExxNaWNyb3NvZnQgVGltZS1TdGFtcCBTZXJ2aWNloIIR+zCCBygwggUQoAMC
# AQICEzMAAAITsEM1Zs+vlegAAQAAAhMwDQYJKoZIhvcNAQELBQAwfDELMAkGA1UE
# BhMCVVMxEzARBgNVBAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1vbmQxHjAc
# BgNVBAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjEmMCQGA1UEAxMdTWljcm9zb2Z0
# IFRpbWUtU3RhbXAgUENBIDIwMTAwHhcNMjUwODE0MTg0ODE3WhcNMjYxMTEzMTg0
# ODE3WjCB0zELMAkGA1UEBhMCVVMxEzARBgNVBAgTCldhc2hpbmd0b24xEDAOBgNV
# BAcTB1JlZG1vbmQxHjAcBgNVBAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjEtMCsG
# A1UECxMkTWljcm9zb2Z0IElyZWxhbmQgT3BlcmF0aW9ucyBMaW1pdGVkMScwJQYD
# VQQLEx5uU2hpZWxkIFRTUyBFU046MzYwNS0wNUUwLUQ5NDcxJTAjBgNVBAMTHE1p
# Y3Jvc29mdCBUaW1lLVN0YW1wIFNlcnZpY2UwggIiMA0GCSqGSIb3DQEBAQUAA4IC
# DwAwggIKAoICAQD0mXrguhnEMg1IWDP70pLk7O/mbnjx49XNz1FdZ7hPj8ymV+Br
# h6rXZEZ2nlxW+eN17m/F+rZrH+Oe7u9Rbitk3iY5Sbm+H6RxixCVhDncXCAgHecS
# NxAeiasbeZl7+jOMVICvoluCUq0h4DJI/MBwXPIB6vmUs1QcES9AwzwE6MzJqkK+
# HTGyDjEoVxUQlAsoR8IYF98xkj9qa60cVvcJRNntpWkbYocQVQ2VnW/Awq/FdM9E
# OdvA8bPLKoknOd+ws0dDi9e3a21LU94KgYjSE3U96rzIawhcz2ihzALToMY1Iz/g
# sDHa4q/CZSfo3AtzT62a+fLrDbytkt6OyRF+dVah8S/WZZjSMdScevBIYFLyBU/2
# BwGzo/mDQ6kk8x/F1SQddGRww89bSEg/w1tbxblK6nwe7CdIpuOnICUYFR0z9Xmt
# lvSxmaSfvXivpQsYr5wssA3pHcWFfo3SePrgXbstMrYFtLSkllpeOjR4M3PVBzF4
# gUtSAX5EGwtgOfwTxwKR7Erw2W3caL3Ml/nnDpR9Nn6TBMzEyoXGHv5N/Hv5oE5t
# n6fH3rUC2KoDLvNVXr2j8tZF0o9l29mf0RLIZtOc9+OQERG/bamtKUROVHDM/puY
# RU4pYtZXDG7CHttRZS5RvVyP3fO+21BgZBq3kT0Assk2aW8soKyQHutouwIDAQAB
# o4IBSTCCAUUwHQYDVR0OBBYEFBOeEErH4WvKmFBYxGKkfj2wwUA6MB8GA1UdIwQY
# MBaAFJ+nFV0AXmJdg/Tl0mWnG1M1GelyMF8GA1UdHwRYMFYwVKBSoFCGTmh0dHA6
# Ly93d3cubWljcm9zb2Z0LmNvbS9wa2lvcHMvY3JsL01pY3Jvc29mdCUyMFRpbWUt
# U3RhbXAlMjBQQ0ElMjAyMDEwKDEpLmNybDBsBggrBgEFBQcBAQRgMF4wXAYIKwYB
# BQUHMAKGUGh0dHA6Ly93d3cubWljcm9zb2Z0LmNvbS9wa2lvcHMvY2VydHMvTWlj
# cm9zb2Z0JTIwVGltZS1TdGFtcCUyMFBDQSUyMDIwMTAoMSkuY3J0MAwGA1UdEwEB
# /wQCMAAwFgYDVR0lAQH/BAwwCgYIKwYBBQUHAwgwDgYDVR0PAQH/BAQDAgeAMA0G
# CSqGSIb3DQEBCwUAA4ICAQCCbFomsapDYPpQmFnpCXZJkU5o24ZtbcvMH4RL6XYE
# HUwm0FFIV2L+FVjfc2nGwlCFDlMtWnQNdg6Qig9BzXusf4hWF6Y7yMK35TojVMjD
# pxHtz60Sj8mOnoSoRTVzj+atoyOAeFD6toL85QCb3wDWvhsg8e2wGYtE4aZ4Tlcs
# gVoEhlYe+HYI5chMo5tdV3nAa0nV1ll3BocAJcXnTqO1r66hR3LMB642VM8tOtny
# fKHEbCT1WHp6INDsJAxZJJrwMlL09ReN6iL29N1Ltkxeq762/pDPfG2gEXn5gUri
# 4T6aIaz3QXGbRUraVauYWGORGXnPKgc53Abuyk1iQOiYI81Yi51RCZBgqm38eyyl
# 9xv7GmdYgNB0zOATymPW+nAuBYScfsu1Ph1kJ6gOj08rjRHEEPyQonvr2eCQTB/A
# IPYRf8xCTv14i86GmcfXYa5UHK9opmTldm+q08403Cvyr+oDfzvsi5bBaCdp5f6m
# unDR1n9Au1sYZWuA/5NFCO37Z1xkDk/dfgvAA2GI+zLQ6XhcJ2Ps7EEsW87OwI8M
# 9pWeSn518MUb404GKvtqpMnrzrbanKaDVX7qBz/VG/EL/CC9jIbTfd5wmq/Q6fRl
# E1iv6L86TCADcc/VosPRoesSnDqW3TbreJGQK+tx1w5bzDeMLxMm5oZbILZL2MSP
# ODCCB3EwggVZoAMCAQICEzMAAAAVxedrngKbSZkAAAAAABUwDQYJKoZIhvcNAQEL
# BQAwgYgxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQH
# EwdSZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xMjAwBgNV
# BAMTKU1pY3Jvc29mdCBSb290IENlcnRpZmljYXRlIEF1dGhvcml0eSAyMDEwMB4X
# DTIxMDkzMDE4MjIyNVoXDTMwMDkzMDE4MzIyNVowfDELMAkGA1UEBhMCVVMxEzAR
# BgNVBAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1vbmQxHjAcBgNVBAoTFU1p
# Y3Jvc29mdCBDb3Jwb3JhdGlvbjEmMCQGA1UEAxMdTWljcm9zb2Z0IFRpbWUtU3Rh
# bXAgUENBIDIwMTAwggIiMA0GCSqGSIb3DQEBAQUAA4ICDwAwggIKAoICAQDk4aZM
# 57RyIQt5osvXJHm9DtWC0/3unAcH0qlsTnXIyjVX9gF/bErg4r25PhdgM/9cT8dm
# 95VTcVrifkpa/rg2Z4VGIwy1jRPPdzLAEBjoYH1qUoNEt6aORmsHFPPFdvWGUNzB
# RMhxXFExN6AKOG6N7dcP2CZTfDlhAnrEqv1yaa8dq6z2Nr41JmTamDu6GnszrYBb
# fowQHJ1S/rboYiXcag/PXfT+jlPP1uyFVk3v3byNpOORj7I5LFGc6XBpDco2LXCO
# Mcg1KL3jtIckw+DJj361VI/c+gVVmG1oO5pGve2krnopN6zL64NF50ZuyjLVwIYw
# XE8s4mKyzbnijYjklqwBSru+cakXW2dg3viSkR4dPf0gz3N9QZpGdc3EXzTdEonW
# /aUgfX782Z5F37ZyL9t9X4C626p+Nuw2TPYrbqgSUei/BQOj0XOmTTd0lBw0gg/w
# EPK3Rxjtp+iZfD9M269ewvPV2HM9Q07BMzlMjgK8QmguEOqEUUbi0b1qGFphAXPK
# Z6Je1yh2AuIzGHLXpyDwwvoSCtdjbwzJNmSLW6CmgyFdXzB0kZSU2LlQ+QuJYfM2
# BjUYhEfb3BvR/bLUHMVr9lxSUV0S2yW6r1AFemzFER1y7435UsSFF5PAPBXbGjfH
# CBUYP3irRbb1Hode2o+eFnJpxq57t7c+auIurQIDAQABo4IB3TCCAdkwEgYJKwYB
# BAGCNxUBBAUCAwEAATAjBgkrBgEEAYI3FQIEFgQUKqdS/mTEmr6CkTxGNSnPEP8v
# BO4wHQYDVR0OBBYEFJ+nFV0AXmJdg/Tl0mWnG1M1GelyMFwGA1UdIARVMFMwUQYM
# KwYBBAGCN0yDfQEBMEEwPwYIKwYBBQUHAgEWM2h0dHA6Ly93d3cubWljcm9zb2Z0
# LmNvbS9wa2lvcHMvRG9jcy9SZXBvc2l0b3J5Lmh0bTATBgNVHSUEDDAKBggrBgEF
# BQcDCDAZBgkrBgEEAYI3FAIEDB4KAFMAdQBiAEMAQTALBgNVHQ8EBAMCAYYwDwYD
# VR0TAQH/BAUwAwEB/zAfBgNVHSMEGDAWgBTV9lbLj+iiXGJo0T2UkFvXzpoYxDBW
# BgNVHR8ETzBNMEugSaBHhkVodHRwOi8vY3JsLm1pY3Jvc29mdC5jb20vcGtpL2Ny
# bC9wcm9kdWN0cy9NaWNSb29DZXJBdXRfMjAxMC0wNi0yMy5jcmwwWgYIKwYBBQUH
# AQEETjBMMEoGCCsGAQUFBzAChj5odHRwOi8vd3d3Lm1pY3Jvc29mdC5jb20vcGtp
# L2NlcnRzL01pY1Jvb0NlckF1dF8yMDEwLTA2LTIzLmNydDANBgkqhkiG9w0BAQsF
# AAOCAgEAnVV9/Cqt4SwfZwExJFvhnnJL/Klv6lwUtj5OR2R4sQaTlz0xM7U518Jx
# Nj/aZGx80HU5bbsPMeTCj/ts0aGUGCLu6WZnOlNN3Zi6th542DYunKmCVgADsAW+
# iehp4LoJ7nvfam++Kctu2D9IdQHZGN5tggz1bSNU5HhTdSRXud2f8449xvNo32X2
# pFaq95W2KFUn0CS9QKC/GbYSEhFdPSfgQJY4rPf5KYnDvBewVIVCs/wMnosZiefw
# C2qBwoEZQhlSdYo2wh3DYXMuLGt7bj8sCXgU6ZGyqVvfSaN0DLzskYDSPeZKPmY7
# T7uG+jIa2Zb0j/aRAfbOxnT99kxybxCrdTDFNLB62FD+CljdQDzHVG2dY3RILLFO
# Ry3BFARxv2T5JL5zbcqOCb2zAVdJVGTZc9d/HltEAY5aGZFrDZ+kKNxnGSgkujhL
# mm77IVRrakURR6nxt67I6IleT53S0Ex2tVdUCbFpAUR+fKFhbHP+CrvsQWY9af3L
# wUFJfn6Tvsv4O+S3Fb+0zj6lMVGEvL8CwYKiexcdFYmNcP7ntdAoGokLjzbaukz5
# m/8K6TT4JDVnK+ANuOaMmdbhIurwJ0I9JZTmdHRbatGePu1+oDEzfbzL6Xu/OHBE
# 0ZDxyKs6ijoIYn/ZcGNTTY3ugm2lBRDBcQZqELQdVTNYs6FwZvKhggNWMIICPgIB
# ATCCAQGhgdmkgdYwgdMxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9u
# MRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRp
# b24xLTArBgNVBAsTJE1pY3Jvc29mdCBJcmVsYW5kIE9wZXJhdGlvbnMgTGltaXRl
# ZDEnMCUGA1UECxMeblNoaWVsZCBUU1MgRVNOOjM2MDUtMDVFMC1EOTQ3MSUwIwYD
# VQQDExxNaWNyb3NvZnQgVGltZS1TdGFtcCBTZXJ2aWNloiMKAQEwBwYFKw4DAhoD
# FQCYETxIKPGCNpybLz9UR2Ts3GlHpqCBgzCBgKR+MHwxCzAJBgNVBAYTAlVTMRMw
# EQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVN
# aWNyb3NvZnQgQ29ycG9yYXRpb24xJjAkBgNVBAMTHU1pY3Jvc29mdCBUaW1lLVN0
# YW1wIFBDQSAyMDEwMA0GCSqGSIb3DQEBCwUAAgUA7oLDWDAiGA8yMDI2MTAyMTA0
# NTEzNloYDzIwMjYxMDIyMDQ1MTM2WjB0MDoGCisGAQQBhFkKBAExLDAqMAoCBQDu
# gsNYAgEAMAcCAQACAg7zMAcCAQACAhIcMAoCBQDuhBTYAgEAMDYGCisGAQQBhFkK
# BAIxKDAmMAwGCisGAQQBhFkKAwKgCjAIAgEAAgMHoSChCjAIAgEAAgMBhqAwDQYJ
# KoZIhvcNAQELBQADggEBAHz96CMV9Nfg18s2RSUxguAU4tUdCqTaQkg64c0LBZi5
# g6eEQTVYlvtSGX01WRE6s8zbLdXlqlpdA+F4vbK/dD6tMchvDhR6P7Q25fCp+Jpj
# VrTySusSxjJA4w1smYiSgsEIS8dL6enOpRT7bSuKgnmm6Kp2AXTUtTAvxu6o+W6V
# nYRbZ7TpSBcd67nJTX1MMLp7sukfa+jJi6cRGM7LqfkCGqdAOlthJ2CPKSNDD0MI
# Pe4oQHEamg8pGu4qpyF/h1MkT8lbuj4o8XTW+GwTvMjuvZ/LzdzR2U1Iml1c6cM6
# falqL0dyJKV3dxfUqeiC/qxOBt82biZmz5xxV6qup1kxggQNMIIECQIBATCBkzB8
# MQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVk
# bW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMSYwJAYDVQQDEx1N
# aWNyb3NvZnQgVGltZS1TdGFtcCBQQ0EgMjAxMAITMwAAAhOwQzVmz6+V6AABAAAC
# EzANBglghkgBZQMEAgEFAKCCAUowGgYJKoZIhvcNAQkDMQ0GCyqGSIb3DQEJEAEE
# MC8GCSqGSIb3DQEJBDEiBCCgWWYyRvlG5R9ahbI/mvtB58+/3BL7n76t6aqShz5x
# KzCB+gYLKoZIhvcNAQkQAi8xgeowgecwgeQwgb0EIMzhCW0UhTPwngOMDM/idWh1
# m9DFgaV5Qh+nzo5rnFhoMIGYMIGApH4wfDELMAkGA1UEBhMCVVMxEzARBgNVBAgT
# Cldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1vbmQxHjAcBgNVBAoTFU1pY3Jvc29m
# dCBDb3Jwb3JhdGlvbjEmMCQGA1UEAxMdTWljcm9zb2Z0IFRpbWUtU3RhbXAgUENB
# IDIwMTACEzMAAAITsEM1Zs+vlegAAQAAAhMwIgQg7giQtffoDX7dCPsExpdGZa2C
# u/tirFg/1Llc0hdiEQwwDQYJKoZIhvcNAQELBQAEggIANcRme9LFMda40anolWqB
# AQB6tZKH7AlYkBeNO8xiRT7v9azqBx4+5RNPc+AUIqoI7o5ygAYxKuA5AjVGjFyh
# ZS2kbX1+fTgUrqHFuOCSnET6tJQ5IgBog3X4xGvewIHzAyOgtLqX3i8vI+dIbCHu
# X3GWDU8JQ3jr+ZqnBtE2JWwt6VvU3+vy88ClnXmcCF0cIdzpcpi6mRf4/7nBitVc
# BjIOX+06IYko2QjPZaIjA72+8ssm+c/gJPvm6pBU7zR6wJvZy4lwuyrEEmD6xxQo
# cWIMhvU3Y384rv9fOEFFxJ82OqpJGOn6KZ85wFTGGTmE53arY0A89RtseiPmfDbI
# U2gO3cE9fB76xSCMIKf4sfhUgyBd0tHrduZBCwTqcO06dLEslbG9wAr6BnhTDTUZ
# xPfJLnD3JpQMAktogToAfPt+xuxAXRx7U129cow8a5Xz8zc5+QlJLN5flQZ4U8h8
# 8OEvleGsiX2GGLFHjqhmsU+/4uFWBWq9UdVP9/+8ijVPNToLsDH+2yhd17qi6xXy
# E1w+V73NXhJV1qOXhxjqq+wB2p1yYjNvPK6IlD/4/tlr6j3CqtioMJIvUnI04Kt1
# Nx+sjecUoWeUFQ/rGDERz7CqQf5ft4A0D/E6cIuYePF7z8+37csS63hR2WqBuO8l
# yBMtCtMW6F3NjtQvJGO3NsY=
# SIG # End signature block
