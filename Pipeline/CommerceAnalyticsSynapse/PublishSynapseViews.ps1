# Publish SQL Server views to Synapse SQL on-demand

param
(
	[switch] $sqlPublish = $true,

	# e.g. cditestsynapse-ondemand.sql.azuresynapse.net
	[Parameter(Mandatory=$true)]
    [string] $serverName,
	
	# password for sqladminuser
	[Parameter(Mandatory=$true)]
	[string] $password,
	
	# e.g. cditestdatalake
	[Parameter(Mandatory=$true)]
	[string] $storageAccount,
	
	# e.g. dynamics365-financeandoperations
	[Parameter(Mandatory=$true)]
	[string] $containerName,
	
	# e.g. commerce-insights-sba5cf56b872844486.cloud.retail.test.dynamics.com
	[Parameter(Mandatory=$true)]
	[string] $datarootpath,
	
	[string] $databaseName = "CommerceAnalytics",
    [string] $userName = "sqladminuser",
    [string] $viewsFolder = ".\Synapse Views",
	[string] $fileFilter = "view_*.sql",
    [string] $fromPath = "https://placeholder_storageaccount.dfs.core.windows.net/placeholder_container/placeholder_datarootpath/" 
)

#Install-Module sqlserver

$pathExists = (Test-Path -Path $viewsFolder)
if ($pathExists)
{
    Write-Host "Start" -ForegroundColor Green
	
	$toPath = "https://$($storageAccount).dfs.core.windows.net/$($containerName)/$($datarootpath)/"
	Write-Host $toPath

    $folder = Get-Item -Path $viewsFolder
    $files = Get-ChildItem $folder.FullName -Recurse -Filter $fileFilter
    ForEach ($file in $files)
    {
        Write-Host $file.Name -ForegroundColor Yellow
        if ($sqlPublish)
        {
            $sqlCmd = [IO.File]::ReadAllText($file.FullName)
            if ($toPath -ne "")
            {
                $sqlCmd = $sqlCmd.Replace($fromPath, $toPath)
            }
            Write-Host "> invoke @ $($databaseName)"
            Invoke-Sqlcmd -ServerInstance $serverName -Database $databaseName -Username $userName -Password $password -Query $sqlCmd
        }
    }
}

Write-Host "End"  -ForegroundColor Green
