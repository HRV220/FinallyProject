FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

# csproj copy (used by docker build context = solution root)
COPY ["src/Core/Core.Domain/Core.Domain.csproj",          "src/Core/Core.Domain/"]
COPY ["src/Core/Core.Infrastructure/Core.Infrastructure.csproj", "src/Core/Core.Infrastructure/"]
COPY ["src/Users/Users.Domain/Users.Domain.csproj",                 "src/Users/Users.Domain/"]
COPY ["src/Users/Users.Application/Users.Application.csproj",       "src/Users/Users.Application/"]
COPY ["src/Users/Users.Infrastructure/Users.Infrastructure.csproj", "src/Users/Users.Infrastructure/"]
COPY ["src/Users/Users.API/Users.API.csproj",                       "src/Users/Users.API/"]

RUN dotnet restore "src/Users/Users.API/Users.API.csproj" -r linux-musl-x64

COPY src/ src/
RUN dotnet publish "src/Users/Users.API/Users.API.csproj" \
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
ENTRYPOINT ["dotnet", "Users.API.dll"]
