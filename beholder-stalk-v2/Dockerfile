FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
ARG TARGETPLATFORM
WORKDIR /src
COPY ["./beholder-stalk-v2.csproj", "."]
COPY ./docker-restore-xplat.sh .
RUN ./docker-restore-xplat.sh
COPY . .
RUN ./docker-build-xplat.sh

FROM build AS publish
ARG TARGETPLATFORM
RUN ./docker-publish-xplat.sh

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS final
ARG TARGETPLATFORM
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "beholder-stalk-v2.dll"]
