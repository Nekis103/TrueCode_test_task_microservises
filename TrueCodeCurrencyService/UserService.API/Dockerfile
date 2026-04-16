FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем все необходимые проекты
COPY SharedKernel/ SharedKernel/
COPY UserService.API/ UserService.API/
COPY UserService.Application/ UserService.Application/
COPY UserService.Domain/ UserService.Domain/
COPY UserService.Infrastructure/ UserService.Infrastructure/

WORKDIR /src/UserService.API
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "UserService.API.dll"]