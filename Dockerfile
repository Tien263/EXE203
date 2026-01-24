# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["Exe_Demo.csproj", "./"]
RUN dotnet restore "Exe_Demo.csproj"

# Copy the rest of the source code
COPY . .

# NUCLEAR FIX: Isolate the real Program.cs and nuke everything else resembling it
RUN ls -la
RUN cp Program.cs Keep.cs
RUN find . -maxdepth 1 -name "Program*" -delete
RUN mv Keep.cs Program.cs
# Also clean up any potential src subdirectory recursion
RUN rm -rf src/Program.csec* || true

# Verify state
RUN echo "Final file list:" && ls -la

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
