# Build script for FPT Alumni Connect API (PowerShell)

Write-Host "Building FPT Alumni Connect API Docker image..." -ForegroundColor Green

# Build the Docker image
docker build -t fptalumniconnect-api:latest .

if ($LASTEXITCODE -eq 0) {
    Write-Host "Build completed successfully!" -ForegroundColor Green
    Write-Host "To run the application, use: docker-compose up -d" -ForegroundColor Yellow
} else {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
} 