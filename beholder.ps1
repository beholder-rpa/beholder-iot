#!/usr/bin/env pwsh
[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [ValidateNotNullOrEmpty()]
    [string] $command,
    [Parameter()]
    [ValidateNotNullOrEmpty()]
    [ValidateSet('dev','arm32v7','arm64')]
    [string] $environment = "dev",
    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]]$args
)

# If /proc/cpuinfo exists, obtain the model and determine if it is a RPi
if ($environment -ne "arm32v7" -and $environment -ne "arm64" -and (Test-Path "/proc/cpuinfo")) {

    $model = (Get-Content "/proc/cpuinfo" | Select-String -Pattern "^\s*?Model\s+?:\s+?(.*?)$").Matches[0].Groups[1].Value
    $bits = (& getconf LONG_BIT)
    # If the model string starts with Raspberry Pi then we're an RPi
    if ($model.StartsWith("Raspberry Pi")) {
      if ($bits -eq 32) {
        Write-Host "Using arm32v7"
        $environment = "arm32v7"
      } elseif ($bits -eq 64) {
        Write-Host "Using arm64"
        $environment = "arm64"
      } else {
        Write-Error "Unable to determine CPU architecture"
        return;
      }
        
    } else {
      throw "Unknown device model: $model"
    }
}

$dockerComposeFiles = @("docker-compose.yml", "docker-compose.$environment.yml")

$hostName = [System.Net.Dns]::GetHostName()
$env:BEHOLDER_HOSTNAME = $hostName
$env:BEHOLDER_SHORT_HOSTNAME = $hostName.Split(".")[0]

if ($environment -eq "arm32v7" -or $environment -eq "arm64") {  
  $env:BEHOLDER_CORTEX_HOSTNAME = $hostName
  $env:BEHOLDER_CEREBRUM_HOSTNAME = "cerebrum.$hostName"
  $env:BEHOLDER_TRAEFIK_HOSTNAME = "traefik.$hostName"
  $env:BEHOLDER_NEXUS_HOSTNAME = "nexus.$hostName"
  $env:BEHOLDER_GRAFANA_HOSTNAME = "grafana.$hostName"
  $env:BEHOLDER_JAEGER_HOSTNAME = "jaeger.$hostName"
}

switch ($command)
{
    'build'
    {
      ## Create the self-signed certificates if they don't already exist
      Push-Location $PWD/beholder-traefik/certs
      if ($IsWindows) {
        Set-ExecutionPolicy Bypass -Scope Process -Force -ErrorAction SilentlyContinue; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; & ./makecerts.ps1;
      } else {
        switch ($environment)
        {
          'dev'
          {
            & $PWD/makecerts.ps1;
          }
          { @("arm32v7", "arm64") -contains $_ }
          {
            & $PWD/makecerts.ps1 -domainsList @("$hostName", ,"*.$hostName");
          }
        }
      }
      Pop-Location

      $dockerComposeCommand = "& docker-compose -f $($dockerComposeFiles -join " -f ") build"
      Invoke-Expression $dockerComposeCommand
    }
    'up'
    {
      # Write out null values for the hidg files
      if ($environment -eq "dev") {
        $null > ./usb-dev/hidg0
        $null > ./usb-dev/hidg1
        $null > ./usb-dev/hidg2
      }

      $dockerComposeCommand = "& docker-compose -f $($dockerComposeFiles -join " -f ") up -d"
      Invoke-Expression $dockerComposeCommand
    }
    'down'
    {
      $dockerComposeCommand = "& docker-compose -f $($dockerComposeFiles -join " -f ") down --remove-orphans"
      Invoke-Expression $dockerComposeCommand
    }
    'pull'
    {
      $dockerComposeCommand = "& docker-compose -f $($dockerComposeFiles -join " -f ") pull"
      Invoke-Expression $dockerComposeCommand
    }
    'clean'
    {
      $currentFolderName = (Split-Path -Path $pwd -Leaf).ToLower()
      docker volume rm "$currentFolderName`_beholder_cortex_node_modules"
      docker volume rm "$currentFolderName`_beholder_cortex_next"
      docker volume rm "$currentFolderName`_beholder_redis_data"
      docker volume rm "$currentFolderName`_beholder_grafana_data"
      docker volume rm "$currentFolderName`_beholder_postgres_data"
    }
    'stop'
    {
      sudo systemctl stop beholder_docker.service
    }
    'start'
    {
      sudo systemctl daemon-reload
      sudo systemctl start beholder_docker.service
    }
    'logs'
    {
      & journalctl -u beholder_docker.service
    }
    'reset-cerebrum'
    {
      $dockerComposeCommand = "& docker-compose -f $($dockerComposeFiles -join " -f ") rm -f -s beholder-cerebrum"
      Invoke-Expression $dockerComposeCommand
      
      docker volume rm beholder_cerebrum_data

      $dockerComposeCommand = "& docker-compose -f $($dockerComposeFiles -join " -f ") up -d"
      Invoke-Expression $dockerComposeCommand
    }
    default
    {
      throw "Unknown command: $command"
    }
}