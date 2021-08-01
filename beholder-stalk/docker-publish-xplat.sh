if [ "$TARGETPLATFORM" = "linux/arm64" ]; then
    echo "Targeting linux/arm64";
    dotnet publish "beholder-stalk.csproj" -c Release -o /app/publish --runtime linux-arm64
elif [ "$TARGETPLATFORM" = "linux/arm/v7" ]; then
    echo "Targeting linux/arm/v7";
    dotnet publish "beholder-stalk.csproj" -c Release -o /app/publish --runtime linux-arm
elif [ "$TARGETPLATFORM" = "linux/amd64" ]; then
    echo "Targeting linux/amd64";
    dotnet publish "beholder-stalk.csproj" -c Release -o /app/publish --runtime linux-x64
else 
    echo "Targeting generic platform ($TARGETPLATFORM)";
    dotnet publish "beholder-stalk.csproj" -c Release -o /app/publish
fi