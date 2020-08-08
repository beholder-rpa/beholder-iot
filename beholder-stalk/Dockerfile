FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["./beholder-stalk.csproj", "."]
RUN dotnet restore "./beholder-stalk.csproj"
COPY . .
RUN dotnet build "beholder-stalk.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "beholder-stalk.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "beholder-stalk.dll"]
