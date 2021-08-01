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
  $hostName = $hostName.Split(".")[0]
  $env:BEHOLDER_CORTEX_HOSTNAME = $hostName
  $env:BEHOLDER_NEXUS_HOSTNAME = "nexus.$hostName"
}

switch ($command)
{
    'build'
    {
      ## Create the self-signed certificates if they don't already exist
      Push-Location $PWD/beholder-traefik/certs
      if ($PSVersionTable.Platform -eq "Windows") {
        Set-ExecutionPolicy Bypass -Scope Process -Force -ErrorAction SilentlyContinue; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; & ./makecerts.ps1;
      } else {
        & $PWD/makecerts.ps1;
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
    'rpi-stop'
    {
      sudo systemctl stop beholder_docker.service
    }
    'rpi-start'
    {
      sudo systemctl start beholder_docker.service
    }
    'rpi-logs'
    {
      & journalctl -u beholder_docker.service
    }
}