FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

COPY ["src/BuildingBlocks/Core.Domain/Core.Domain.csproj",                              "src/BuildingBlocks/Core.Domain/"]
COPY ["src/BuildingBlocks/Core.Infrastructure/Core.Infrastructure.csproj",              "src/BuildingBlocks/Core.Infrastructure/"]
COPY ["src/Transactions/Transactions.Domain/Transactions.Domain.csproj",                "src/Transactions/Transactions.Domain/"]
COPY ["src/Transactions/Transactions.Application/Transactions.Application.csproj",      "src/Transactions/Transactions.Application/"]
COPY ["src/Transactions/Transactions.Infrastructure/Transactions.Infrastructure.csproj","src/Transactions/Transactions.Infrastructure/"]
COPY ["src/Transactions/Transactions.API/Transactions.API.csproj",                      "src/Transactions/Transactions.API/"]

RUN dotnet restore "src/Transactions/Transactions.API/Transactions.API.csproj" -r linux-musl-x64

COPY src/ src/
RUN dotnet publish "src/Transactions/Transactions.API/Transactions.API.csproj" \
    -c Release \
    -r linux-musl-x64 \
    --no-self-contained \
    --no-restore \
    -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime
RUN apk add --no-cache icu-libs
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Transactions.API.dll"]
