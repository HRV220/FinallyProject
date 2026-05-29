FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

COPY ["src/Core/Core.Domain/Core.Domain.csproj",                           "src/Core/Core.Domain/"]
COPY ["src/Core/Core.Infrastructure/Core.Infrastructure.csproj",           "src/Core/Core.Infrastructure/"]
COPY ["src/Categories/Categories.Domain/Categories.Domain.csproj",                   "src/Categories/Categories.Domain/"]
COPY ["src/Categories/Categories.Application/Categories.Application.csproj",         "src/Categories/Categories.Application/"]
COPY ["src/Categories/Categories.Infrastructure/Categories.Infrastructure.csproj",   "src/Categories/Categories.Infrastructure/"]
COPY ["src/Categories/Categories.API/Categories.API.csproj",                         "src/Categories/Categories.API/"]

RUN dotnet restore "src/Categories/Categories.API/Categories.API.csproj" -r linux-musl-x64

COPY src/ src/
RUN dotnet publish "src/Categories/Categories.API/Categories.API.csproj" \
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
ENTRYPOINT ["dotnet", "Categories.API.dll"]
