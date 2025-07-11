# Stop script for FPT Alumni Connect API (PowerShell)

Write-Host "Stopping FPT Alumni Connect API..." -ForegroundColor Yellow

# Stop and remove containers
docker-compose down

if ($LASTEXITCODE -eq 0) {
    Write-Host "Application stopped successfully!" -ForegroundColor Green
} else {
    Write-Host "Failed to stop application!" -ForegroundColor Red
    exit 1
} 