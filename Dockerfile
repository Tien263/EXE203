# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["Exe_Demo.csproj", "./"]
RUN dotnet restore "Exe_Demo.csproj"

# Copy only necessary folders (Whitelist approach to avoid junk files)
COPY Program.cs .
COPY appsettings.json .
COPY Controllers/ ./Controllers/
COPY Data/ ./Data/
COPY Database/ ./Database/
COPY Helpers/ ./Helpers/
COPY Migrations/ ./Migrations/
COPY Models/ ./Models/
COPY Repositories/ ./Repositories/
COPY Services/ ./Services/
COPY Views/ ./Views/
COPY wwwroot/ ./wwwroot/

# Build the application
RUN dotnet build "Exe_Demo.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "Exe_Demo.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
# Ensure we run as root to have write permissions for mounted volumes
USER root
WORKDIR /app
COPY --from=publish /app/publish .

# Environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV PORT=8080
EXPOSE 8080

# Run the application
ENTRYPOINT ["dotnet", "Exe_Demo.dll"]
