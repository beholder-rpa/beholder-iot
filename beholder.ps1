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

$dockerComposeFiles = @("docker-compose.yml", "docker-compose.dev.yml")
if ($environment -eq "rpi") {
  $dockerComposeFiles = @("docker-compose.yml", "docker-compose.rpi.yml")
}

if ($environment -eq "rpi") {
  $hostName = [System.Net.Dns]::GetHostName()
  $env:BEHOLDER_HOSTNAME = $hostName
  $env:BEHOLDER_CORTEX_HOSTNAME = $hostName
  $env:BEHOLDER_NEXUS_HOSTNAME = "nexus.$hostName"
  $env:BEHOLDER_GRAFANA_HOSTNAME = "grafana.$hostName"
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
            & $PWD/makecerts.ps1 -domainsList @("$hostName", ,"nexus.$hostName","grafana.$hostName");
          }
        }
      }
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