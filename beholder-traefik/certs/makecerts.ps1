#!/usr/bin/env pwsh

[CmdletBinding()]
param(
    [Parameter()]
    [SecureString] $certificatePassword = (ConvertTo-SecureString -String "mUSn4LwfTb2jOXM3ftw4VKck4XGlRMLf" -AsPlainText -Force),
    [Parameter()]
    [string] $outputPath = $pwd
)

Write-Host "Generating certificates..."

$localDomainsList = @(
    "beholder.localhost",
    "nexus.beholder.localhost",
    "graphana.beholder.localhost"
    )

# If openssl is available, this will be used to generate the certificate.
if (Get-Command "openssl" -ErrorAction SilentlyContinue) 
{
    Write-Host "Using OpenSSL..."

    $passOut = "pass:"
    if ($certificatePassword -ne $null) {
        $passOut = "pass:$(ConvertFrom-SecureString -SecureString $certificatePassword -AsPlainText)"
    }

    foreach ($domain in $localDomainsList) {

        if (-not(Test-Path -Path "$outputPath/$domain.crt" -PathType Leaf)) {

            & openssl req -x509 -sha256 -nodes -days 1825 -newkey rsa:2048 `
                -subj "/C=US/ST=Oregon/L=Portland/O=PDP/OU=Beholder/CN=$domain" `
                -addext "subjectAltName=DNS:$domain,DNS:$domain,IP:127.0.0.1" `
                -keyout "$outputPath/$domain.key" `
                -out "$outputPath/$domain.crt" `
                -passout $passOut
            if ($IsMacOS) {
                & sudo security add-trusted-cert -d -r trustRoot -k /Library/Keychains/System.keychain "$outputPath/$domain.crt"
            }

            if ($IsLinux) {
                & sudo cp "$outputPath/$domain.crt" /usr/local/share/ca-certificates/
            }
        }
    }

    if ($IsLinux) {
        sudo update-ca-certificates --fresh
    }
} else {

    Write-Host "Using mkcert..."

    # If openssl is not available, we'll use makecert certificate generation
    try {
        $mkcert = ".\mkcert.exe"
        if ($null -ne (Get-Command mkcert.exe -ErrorAction SilentlyContinue)) {
            # mkcert installed in PATH
            $mkcert = "mkcert"
        } elseif (-not (Test-Path $mkcert)) {
            Write-Host "Downloading and installing mkcert certificate tool..." -ForegroundColor Green 
            Invoke-WebRequest "https://github.com/FiloSottile/mkcert/releases/download/v1.4.1/mkcert-v1.4.1-windows-amd64.exe" -UseBasicParsing -OutFile mkcert.exe
            if ((Get-FileHash mkcert.exe).Hash -ne "1BE92F598145F61CA67DD9F5C687DFEC17953548D013715FF54067B34D7C3246") {
                Remove-Item mkcert.exe -Force
                throw "Invalid mkcert.exe file"
            }
        }
        Write-Host "Generating Traefik TLS certificates..." -ForegroundColor Green
        & $mkcert -install

        foreach ($domain in $localDomainsList) {
            if (-not(Test-Path -Path "$outputPath/$domain.crt" -PathType Leaf)) {
                & $mkcert -cert-file $outputPath/$domain.crt -key-file $outputPath/$domain.key "$domain"
            }
        }
    }
    catch {
        Write-Host "An error occurred while attempting to generate TLS certificates: $_" -ForegroundColor Red
    }
}


