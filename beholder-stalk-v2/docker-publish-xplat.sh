if [ "$TARGETPLATFORM" = "linux/arm64" ]; then
    echo "Targeting linux/arm64";
    dotnet publish "beholder-stalk-v2.csproj" -c Release -o /app/publish --runtime linux-arm64 --no-restore
elif [ "$TARGETPLATFORM" = "linux/arm/v7" ]; then
    echo "Targeting linux/arm/v7";
    dotnet publish "beholder-stalk-v2.csproj" -c Release -o /app/publish --runtime linux-arm --no-restore
elif [ "$TARGETPLATFORM" = "linux/amd64" ]; then
    echo "Targeting linux/amd64";
    dotnet publish "beholder-stalk-v2.csproj" -c Release -o /app/publish --runtime linux-x64 --no-restore
else 
    echo "Targeting generic platform ($TARGETPLATFORM)";
    dotnet publish "beholder-stalk-v2.csproj" -c Release -o /app/publish --no-restore
fi