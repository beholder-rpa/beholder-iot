if [ "$TARGETPLATFORM" = "linux/arm64" ]; then
    echo "Targeting linux/arm64";
    dotnet restore "./beholder-stalk.csproj" --runtime linux-arm64
elif [ "$TARGETPLATFORM" = "linux/arm/v7" ]; then
    echo "Targeting linux/arm/v7";
    # Sigh... Just microsoft things...
    # https://docs.microsoft.com/en-us/dotnet/core/install/linux-package-mixup
    wget https://download.visualstudio.microsoft.com/download/pr/f456f253-db24-45ea-9c73-f507f93a8cd2/6efe7bed8639344d9c9afb8a46686c99/dotnet-sdk-5.0.302-linux-arm.tar.gz
    wget https://download.visualstudio.microsoft.com/download/pr/7e928c60-5f60-4c62-8439-422be547605c/0d1dc316cf38efdb2557f639ca9da4ad/aspnetcore-runtime-5.0.8-linux-arm.tar.gz
    rm /usr/bin/dotnet
    rm -rf /usr/share/dotnet
    mkdir /usr/share/dotnet
    tar -xvf dotnet-sdk-5.0.302-linux-arm.tar.gz -C /usr/share/dotnet
    tar -xvf aspnetcore-runtime-5.0.8-linux-arm.tar.gz -C /usr/share/dotnet
    ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet
    export DOTNET_ROOT=/usr/share/dotnet/
    ls -l /usr/share/dotnet/host/fxr
    ls -l /usr/share/dotnet/host/fxr/5.0.8
    dotnet --version
    dotnet restore "./beholder-stalk.csproj" --runtime linux-arm
elif [ "$TARGETPLATFORM" = "linux/amd64" ]; then
    echo "Targeting linux/amd64";
    dotnet restore "./beholder-stalk.csproj" --runtime linux-x64
else 
    echo "Targeting generic platform ($TARGETPLATFORM)";
    dotnet restore "./beholder-stalk.csproj" -o /app/build
fi