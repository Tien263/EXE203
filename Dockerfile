# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["Exe_Demo.csproj", "./"]
RUN dotnet restore "Exe_Demo.csproj"

# Copy the rest of the source code
COPY . .

# Emergency fix: Remove junk files causing build errors
RUN rm -f Program.csecProgram.cs src/Program.csecProgram.cs || true
RUN find . -name "*csec*" -delete || true

# Build the application
RUN dotnet build "Exe_Demo.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "Exe_Demo.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV PORT=8080
EXPOSE 8080

# Run the application
ENTRYPOINT ["dotnet", "Exe_Demo.dll"]
