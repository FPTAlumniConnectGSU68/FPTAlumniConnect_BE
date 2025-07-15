# Multi-stage build để tối ưu hóa kích thước image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy project files
COPY ["FPTAlumniConnect.API/FPTAlumniConnect.API.csproj", "FPTAlumniConnect.API/"]
COPY ["FPTAlumniConnect.BusinessTier/FPTAlumniConnect.BusinessTier.csproj", "FPTAlumniConnect.BusinessTier/"]
COPY ["FPTAlumniConnect.DataTier/FPTAlumniConnect.DataTier.csproj", "FPTAlumniConnect.DataTier/"]

# Restore dependencies
RUN dotnet restore "FPTAlumniConnect.API/FPTAlumniConnect.API.csproj"

# Copy source code
COPY . .
WORKDIR "/src/FPTAlumniConnect.API"

# Build application
RUN dotnet build "FPTAlumniConnect.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "FPTAlumniConnect.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create non-root user for security
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "FPTAlumniConnect.API.dll"]
