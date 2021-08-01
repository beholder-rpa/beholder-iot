if [ "$TARGETPLATFORM" = "linux/arm64" ]; then
    echo "Targeting linux/arm64";
    dotnet restore "./beholder-stalk.csproj" --runtime linux-arm64
elif [ "$TARGETPLATFORM" = "linux/arm/v7" ]; then
    echo "Targeting linux/arm/v7";
    # Sigh... Just microsoft things...
    # https://docs.microsoft.com/en-us/dotnet/core/install/linux-package-mixup
    # "A fatal error occurred, the folder [/usr/share/dotnet/host/fxr] does not contain any version-numbered child folders" error will be reported.
    # This is also really useful: https://github.com/dotnet/dotnet-docker/issues/1537#issuecomment-615269150
    # You can not run .NET within arm32 images on amd64 hosts but it is possible to build an .NET arm32 image on an amd64 host using the [this pattern](https://github.com/dotnet/dotnet-docker/blob/master/samples/dotnetapp/Dockerfile.debian-arm32).
    # The subtle difference here is you are using an amd64 SDK image to product the arm32 binaries that get copied to the resulting arm32 image.
    dotnet restore "./beholder-stalk.csproj" --runtime linux-arm
elif [ "$TARGETPLATFORM" = "linux/amd64" ]; then
    echo "Targeting linux/amd64";
    dotnet restore "./beholder-stalk.csproj" --runtime linux-x64
else 
    echo "Targeting generic platform ($TARGETPLATFORM)";
    dotnet restore "./beholder-stalk.csproj" -o /app/build
fi