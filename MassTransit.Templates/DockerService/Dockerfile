FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["DockerService.csproj", "DockerService/"]
RUN dotnet restore "DockerService/DockerService.csproj"

COPY . DockerService/.
RUN dotnet publish -c Release -o /app DockerService/DockerService.csproj 

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS publish
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "DockerService.dll"]