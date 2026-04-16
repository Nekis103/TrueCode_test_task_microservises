FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем все необходимые проекты
COPY SharedKernel/ SharedKernel/
COPY FinanceService.API/ FinanceService.API/
COPY FinanceService.Application/ FinanceService.Application/
COPY FinanceService.Domain/ FinanceService.Domain/
COPY FinanceService.Infrastructure/ FinanceService.Infrastructure/

WORKDIR /src/FinanceService.API
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FinanceService.API.dll"]