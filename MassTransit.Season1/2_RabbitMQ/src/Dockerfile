FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:6.0 AS publish
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "Sample.Service.exe"]