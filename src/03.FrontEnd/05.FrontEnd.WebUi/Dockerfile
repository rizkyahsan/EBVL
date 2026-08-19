# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Enable forwarded headers globally
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true

# Define backend API base URL for container runtime
# This will override appsettings.json when running inside Docker
ENV BackEndApiBaseUrl=https://ebvl-api.example.invalid

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/03.FrontEnd/05.FrontEnd.WebUi/05.FrontEnd.WebUi.csproj", "src/03.FrontEnd/05.FrontEnd.WebUi/"]
COPY ["src/03.FrontEnd/03.FrontEnd.Infrastructure/03.FrontEnd.Infrastructure.csproj", "src/03.FrontEnd/03.FrontEnd.Infrastructure/"]
COPY ["src/01.Shared/03.Shared.Dto/03.Shared.Dto.csproj", "src/01.Shared/03.Shared.Dto/"]
COPY ["src/01.Shared/01.Shared.Enums/01.Shared.Enums.csproj", "src/01.Shared/01.Shared.Enums/"]
COPY ["src/01.Shared/02.Shared.Statics/02.Shared.Statics.csproj", "src/01.Shared/02.Shared.Statics/"]
COPY ["src/03.FrontEnd/02.FrontEnd.Services/02.FrontEnd.Services.csproj", "src/03.FrontEnd/02.FrontEnd.Services/"]
COPY ["src/03.FrontEnd/04.FrontEnd.Logics/04.FrontEnd.Logics.csproj", "src/03.FrontEnd/04.FrontEnd.Logics/"]
RUN --mount=type=secret,id=nuget_pat \
    dotnet nuget add source "https://pkgs.dev.azure.com/pertamina/_packaging/pertamina/nuget/v3/index.json" \
      --name "pertamina" --username "any" --password "$(cat /run/secrets/nuget_pat)" --store-password-in-clear-text \
    && dotnet restore "./src/03.FrontEnd/05.FrontEnd.WebUi/05.FrontEnd.WebUi.csproj"
RUN dotnet restore "./src/03.FrontEnd/05.FrontEnd.WebUi/05.FrontEnd.WebUi.csproj"
COPY . .
WORKDIR "/src/src/03.FrontEnd/05.FrontEnd.WebUi"
RUN dotnet build "./05.FrontEnd.WebUi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./05.FrontEnd.WebUi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EBVL.FrontEnd.WebUi.dll"]
