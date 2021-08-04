#!/usr/bin/env pwsh

[CmdletBinding()]
param(
    [Parameter()]
    [SecureString] $certificatePassword = (ConvertTo-SecureString -String "mUSn4LwfTb2jOXM3ftw4VKck4XGlRMLf" -AsPlainText -Force),
    [Parameter()]
    [string] $outputPath = $pwd,
    [Parameter()]
    [string[]] $domainsList = @("beholder.localhost","nexus.beholder.localhost","grafana.beholder.localhost")
)

Write-Host "Generating certificates..."

$openSSLPath
if ($IsWindows) {
    $openSSLPath = "openssl"
} ElseIf ($IsLinux) {
    $openSSLPath = "openssl"
} ElseIf ($IsMacOS) {
    $openSSLPath = "/usr/local/opt/openssl/bin/openssl"
}

$makeCertPath = "mkcert"
if (!(Get-Command $makeCertPath -ErrorAction SilentlyContinue) -and !(Get-Command $openSSLPath -ErrorAction SilentlyContinue)) {
    # if mkcert is not installed, try to install it
    if ($IsWindows) {
        Write-Host "Downloading and installing mkcert certificate tool..." -ForegroundColor Green 
        Invoke-WebRequest "https://github.com/FiloSottile/mkcert/releases/download/v1.4.3/mkcert-v1.4.3-windows-amd64.exe" -UseBasicParsing -OutFile mkcert.exe
        if ((Get-FileHash mkcert.exe).Hash -ne "9DC25F7D1AE0BE93DB81AA42F3ABFD62D13725DFD48969C9FE94B6AF57E5573C") {
            Remove-Item mkcert.exe -Force
            throw "Invalid mkcert.exe file"
        }

        $makeCertPath = "$PWD\mkcert.exe"
    }
}

if (Get-Command $makeCertPath -ErrorAction SilentlyContinue) {
    Write-Host "Using mkcert..."

    try {
        Write-Host "Generating Traefik TLS certificates..." -ForegroundColor Green
        & $makeCertPath -install

        $cmd = "& $makeCertPath -cert-file `"$outputPath/server.crt`" -key-file `"$outputPath/server.key`" `"$($domainsList -join '" "')`""
        Write-Host $cmd
        Invoke-Expression $cmd
    } catch {
        Write-Host "An error occurred while attempting to generate TLS certificates: $_" -ForegroundColor Red
    }
}

# If openssl is available, this will be used to generate the certificate.
ElseIf (Get-Command $openSSLPath -ErrorAction SilentlyContinue) {
    Write-Host "Using OpenSSL..."

    $passOut = "pass:"
    if ($certificatePassword -ne $null) {
        $passOut = "pass:$(ConvertFrom-SecureString -SecureString $certificatePassword -AsPlainText)"
    }

    try {
    & $openSSLPath req -x509 -sha256 -nodes -new -days 365 -newkey rsa:2048 `
        -subj "/CN=BEHOLDER DEFAULT CERT" `
        -addext "keyUsage=critical,digitalSignature,keyEncipherment,dataEncipherment,keyAgreement" `
        -addext "extendedKeyUsage=serverAuth" `
        -addext "subjectAltName=DNS:$($domainsList -join ",DNS:")" `
        -keyout "$outputPath/server.key" `
        -out "$outputPath/server.crt" `
        -passout $passOut
    } catch {
        Write-Host "An error occurred while attempting to generate TLS certificates: $_" -ForegroundColor Red
    }

    if ($IsMacOS) {
        & sudo security add-trusted-cert -d -r trustRoot -k /Library/Keychains/System.keychain "$outputPath/server.crt"
    } ElseIf ($IsLinux) {
        & sudo cp "$outputPath/server.crt" /usr/local/share/ca-certificates/
        & sudo update-ca-certificates --fresh
    } ElseIf ($IsWindows) {
        & $openSSLPath pkcs12 -export `
                -in "$outputPath/server.crt" `
                -inkey "$outputPath/server.key" `
                -passout "pass:$(ConvertFrom-SecureString -SecureString $certificatePassword -AsPlainText)" `
                -out "$PWD/server.pfx"
        $cmd = "Import-PfxCertificate -FilePath `"$outputPath\server.pfx`" -CertStoreLocation Cert:\LocalMachine\My -Password (ConvertTo-SecureString -String `"$(ConvertFrom-SecureString -SecureString $certificatePassword -AsPlainText)`" -AsPlainText -Force)"
        Write-Host -ForegroundColor yellow "To trust this certificate, please execute the following from an elevated powershell prompt: $cmd"
    }
} else {
    Write-Host "Unable to find mkcert or openssl. Please install mkcert or openssl and try again." -ForegroundColor Red
}
