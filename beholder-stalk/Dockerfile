FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["./beholder-stalk.csproj", "."]
RUN dotnet restore "./beholder-stalk.csproj"
COPY . .
RUN ./docker-build-xplat.sh

FROM build AS publish
RUN ./docker-publish-xplat.sh

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "beholder-stalk.dll"]
