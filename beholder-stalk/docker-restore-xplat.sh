if [ "$TARGETPLATFORM" = "linux/arm64" ]; then
    echo "Targeting linux/arm64";
    dotnet restore "./beholder-stalk.csproj" --runtime linux-arm64
elif [ "$TARGETPLATFORM" = "linux/arm/v7" ]; then
    echo "Targeting linux/arm/v7";
    # Sigh... Just microsoft things...
    # https://docs.microsoft.com/en-us/dotnet/core/install/linux-package-mixup
    apt-get remove -y packages-microsoft-prod
    apt-get remove -y 'dotnet*' 'aspnet*' 'netstandard*'
    apt-get install -y wget
    wget https://packages.microsoft.com/config/debian/10/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
    dpkg -i packages-microsoft-prod.deb
    rm packages-microsoft-prod.deb
    apt-get update; \
        apt-get install -y apt-transport-https && \
        apt-get update && \
        apt-get install -y dotnet-sdk-5.0
    dotnet restore "./beholder-stalk.csproj" --runtime linux-arm
elif [ "$TARGETPLATFORM" = "linux/amd64" ]; then
    echo "Targeting linux/amd64";
    dotnet restore "./beholder-stalk.csproj" --runtime linux-x64
else 
    echo "Targeting generic platform ($TARGETPLATFORM)";
    dotnet restore "./beholder-stalk.csproj" -o /app/build
fi