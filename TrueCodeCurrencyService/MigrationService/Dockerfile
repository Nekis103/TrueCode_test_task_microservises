FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY SharedKernel/ ./SharedKernel/
COPY MigrationService/ ./MigrationService/
COPY UserService.API/ ./UserService.API/
COPY UserService.Application/ ./UserService.Application/
COPY UserService.Domain/ ./UserService.Domain/
COPY UserService.Infrastructure/ ./UserService.Infrastructure/

WORKDIR /src/MigrationService
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# ИСПРАВЛЕНО: используем aspnet вместо runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MigrationService.dll"]