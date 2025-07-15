#!/bin/bash

# Stop script for FPT Alumni Connect API

echo "Stopping FPT Alumni Connect API..."

# Stop and remove containers
docker-compose down

echo "Application stopped successfully!" 