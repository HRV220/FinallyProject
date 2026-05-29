FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

COPY ["src/BuildingBlocks/Core.Domain/Core.Domain.csproj",                "src/BuildingBlocks/Core.Domain/"]
COPY ["src/BuildingBlocks/Core.Infrastructure/Core.Infrastructure.csproj","src/BuildingBlocks/Core.Infrastructure/"]
COPY ["src/Reports/Reports.Domain/Reports.Domain.csproj",                 "src/Reports/Reports.Domain/"]
COPY ["src/Reports/Reports.Application/Reports.Application.csproj",       "src/Reports/Reports.Application/"]
COPY ["src/Reports/Reports.Infrastructure/Reports.Infrastructure.csproj", "src/Reports/Reports.Infrastructure/"]
COPY ["src/Reports/Reports.API/Reports.API.csproj",                       "src/Reports/Reports.API/"]

RUN dotnet restore "src/Reports/Reports.API/Reports.API.csproj" -r linux-musl-x64

COPY src/ src/
RUN dotnet publish "src/Reports/Reports.API/Reports.API.csproj" \
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
ENTRYPOINT ["dotnet", "Reports.API.dll"]
