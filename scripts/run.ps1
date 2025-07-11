# Run script for FPT Alumni Connect API (PowerShell)

Write-Host "Starting FPT Alumni Connect API with Docker Compose..." -ForegroundColor Green

# Check if .env file exists
if (-not (Test-Path ".env")) {
    Write-Host "Creating .env file from template..." -ForegroundColor Yellow
    Copy-Item "env.example" ".env"
    Write-Host "Please update .env file with your configuration values" -ForegroundColor Yellow
}

# Start the application
docker-compose up -d

if ($LASTEXITCODE -eq 0) {
    Write-Host "Application is starting..." -ForegroundColor Green
    Write-Host "API will be available at: http://localhost:5000" -ForegroundColor Cyan
    Write-Host "Swagger UI will be available at: http://localhost:5000/swagger" -ForegroundColor Cyan
    Write-Host "SQL Server will be available at: localhost:1433" -ForegroundColor Cyan
    
    # Show running containers
    docker-compose ps
} else {
    Write-Host "Failed to start application!" -ForegroundColor Red
    exit 1
} 