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

$azureADPreviewmodule = Get-Module 'AzureADPreview' -ListAvailable -ErrorAction SilentlyContinue
$azureADmodule = Get-Module 'AzureAD' -ListAvailable -ErrorAction SilentlyContinue

if (!$azureADPreviewmodule -And !$azureADmodule)
{
Write-Host "Azure AD module is not installed."  -ForegroundColor Yellow
EXIT 1
}

Write-Host "Connecting $TenantId..." -NoNewline;

try
{
 connect-AzureAD -TenantId "$tenantId.onmicrosoft.com" -ErrorAction Stop >$null
}
catch
{
Write-Host $_.Exception.Message -ForegroundColor Yellow
EXIT 1
}

Write-Host -ForegroundColor Green "Done"

$aad = (Get-AzureADServicePrincipal | where {$_.ServicePrincipalNames.Contains("https://graph.windows.net")})[0]

Write-Host "Reading directory write data role..." -NoNewline;

#  Resource Access User.Read + Sign in & Directory.ReadWrite.All
$readWriteAccess = [Microsoft.Open.AzureAD.Model.RequiredResourceAccess]@{
  ResourceAppId="00000003-0000-0000-c000-000000000000" ;
  ResourceAccess = [Microsoft.Open.AzureAD.Model.ResourceAccess]@{
    Id = "37f7f235-527c-4136-accd-4a02d197296e" ;
    Type = "Scope"}, [Microsoft.Open.AzureAD.Model.ResourceAccess]@{
    Id = "7427e0e9-2fba-42fe-b0c0-848c9e6a8182" ;
    Type = "Scope"}}

Write-Host -ForegroundColor Green "Done"

# Creating web application
Write-Host "Creating AzureAD Web application : $webAppDisplayName..." -NoNewline;

try
{
$app = New-AzureADApplication -DisplayName $webAppDisplayName -ReplyUrls $replyUrl -RequiredResourceAccess @($readWriteAccess) -ErrorAction Stop
}
catch
{
Write-Host $_.Exception.Message -ForegroundColor Yellow
EXIT 1
}

Write-Host -ForegroundColor Green "Done"

Write-Host "Creating Service prinicipal..." -NoNewline;

$spSvc = New-AzureADServicePrincipal -AppId $app.AppId

Write-Host -ForegroundColor Green "Done"

Write-Host "Reading directory read data role..." -NoNewline;

#  Grab the User.Read permission
$userRead = $aad.Oauth2Permissions | ? {$_.Value -eq "User.Read"}

#  Resource Access User.Read + Sign in
$readUserAccess = [Microsoft.Open.AzureAD.Model.RequiredResourceAccess]@{
  ResourceAppId=$aad.AppId ;
  ResourceAccess=[Microsoft.Open.AzureAD.Model.ResourceAccess]@{
    Id = $userRead.Id ;
    Type = "Scope"}}

#  Grab the user-impersonation permission
$svcUserImpersonation = $spSvc.Oauth2Permissions | ?{$_.Value -eq "user_impersonation"}

Write-Host -ForegroundColor Green "Done"

$accessAccess = [Microsoft.Open.AzureAD.Model.RequiredResourceAccess]@{
  ResourceAppId=$app.AppId ;
  ResourceAccess=[Microsoft.Open.AzureAD.Model.ResourceAccess]@{
    Id = $svcUserImpersonation.Id ;
    Type = "Scope"}}
    
#Creating native application
Write-Host "Creating AzureAD Native application ($nativeAppDisplayName)..." -NoNewline;
$nativeApp = New-AzureADApplication -DisplayName $nativeAppDisplayName -PublicClient $true -RequiredResourceAccess $readUserAccess,$accessAccess
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
$appObjectId = $app.ObjectId
$proxyAppId = $nativeApp.AppId
$proxyAppObjectId = $nativeApp.ObjectId
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
Write-Host -ForegroundColor Cyan $spSvc.ObjectId

Write-Host "Proxy Application Id : " -NoNewline;
Write-Host -ForegroundColor Cyan $proxyAppId
Add-Content $outputFile  "Native Application Id : $proxyAppId"

Write-Host "Proxy Object Id : " -NoNewline;
Write-Host -ForegroundColor Cyan $nativeApp.ObjectId
Add-Content $outputFile  "Native Application Object Id: $proxyAppObjectId"

Write-Host "Pre-installation is completed."