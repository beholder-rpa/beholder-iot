if [ "$TARGETPLATFORM" = "linux/arm64" ]; then
    echo "Targeting linux/arm64";
    dotnet restore "./beholder-stalk.csproj" --runtime linux-arm64
elif [ "$TARGETPLATFORM" = "linux/arm/v7" ]; then
    echo "Targeting linux/arm/v7";
    # Sigh... Just microsoft things...
    # https://docs.microsoft.com/en-us/dotnet/core/install/linux-package-mixup
    sudo dnf remove packages-microsoft-prod
    sudo dnf remove 'dotnet*' 'aspnet*' 'netstandard*'
    sudo dnf install dotnet-sdk-5.0
    dotnet restore "./beholder-stalk.csproj" --runtime linux-arm
elif [ "$TARGETPLATFORM" = "linux/amd64" ]; then
    echo "Targeting linux/amd64";
    dotnet restore "./beholder-stalk.csproj" --runtime linux-x64
else 
    echo "Targeting generic platform ($TARGETPLATFORM)";
    dotnet restore "./beholder-stalk.csproj" -o /app/build
fi