#!/usr/bin/env pwsh
[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [ValidateNotNullOrEmpty()]
    [string] $command,
    [Parameter()]
    [ValidateNotNullOrEmpty()]
    [ValidateSet('dev','rpi')]
    [string] $environment = "dev"
)

$dockerComposeFiles = @("docker-compose.yml", "docker-compose.$environment.yml")

$hostName = [System.Net.Dns]::GetHostName()
$env:BEHOLDER_HOSTNAME = $hostName
$env:BEHOLDER_SHORT_HOSTNAME = $hostName.Split(".")[0]

if ($environment -eq "rpi") {  
  $env:BEHOLDER_CORTEX_HOSTNAME = $hostName
  $env:BEHOLDER_TRAEFIK_HOSTNAME = "traefik.$hostName"
  $env:BEHOLDER_NEXUS_HOSTNAME = "nexus.$hostName"
  $env:BEHOLDER_GRAFANA_HOSTNAME = "grafana.$hostName"
  $env:BEHOLDER_JAEGER_HOSTNAME = "jaeger.$hostName"
  $env:BEHOLDER_NODERED_HOSTNAME = "nodered.$hostName"
} elseif ($environment -eq "dev") {
  $null > ./usb-dev/hidg0
  $null > ./usb-dev/hidg1
  $null > ./usb-dev/hidg2
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
          'rpi'
          {
            & $PWD/makecerts.ps1 -domainsList @("$hostName", ,"*.$hostName");
          }
        }
      }
      Pop-Location

      ## Install Node-RED modules
      Push-Location $PWD/beholder-node-red/data
      & npm install
      Pop-Location

      $dockerComposeCommand = "& docker-compose -f $($dockerComposeFiles -join " -f ") build"
      Invoke-Expression $dockerComposeCommand
    }
    'up'
    {
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
    default
    {
      throw "Unknown command: $command"
    }
}