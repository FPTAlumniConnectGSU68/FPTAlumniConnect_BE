#!/bin/bash

# Run script for FPT Alumni Connect API

echo "Starting FPT Alumni Connect API with Docker Compose..."

# Check if .env file exists
if [ ! -f .env ]; then
    echo "Creating .env file from template..."
    cp env.example .env
    echo "Please update .env file with your configuration values"
fi

# Start the application
docker-compose up -d

echo "Application is starting..."
echo "API will be available at: http://localhost:5000"
echo "Swagger UI will be available at: http://localhost:5000/swagger"
echo "SQL Server will be available at: localhost:1433"

# Show running containers
docker-compose ps 