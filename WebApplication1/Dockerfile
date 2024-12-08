# Use the official .NET 8 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory inside the container
WORKDIR /app

# Copy the project file and restore any dependencies (via `dotnet restore`)
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the files and publish the app
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Define the runtime image (to run the application)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
COPY --from=build /app/publish .

# Set the entry point for the container (when the container starts)
ENTRYPOINT ["dotnet", "WebApplication1.dll"]
