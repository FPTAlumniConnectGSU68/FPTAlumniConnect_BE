# FPT Alumni Connect - Docker Setup

## Yêu cầu hệ thống

- Docker Desktop
- Docker Compose
- WSL2 (Windows)

## Cài đặt và chạy ứng dụng

### 1. Clone repository
```bash
git clone <repository-url>
cd FPTAlumniConnect_BE
```

### 2. Cấu hình môi trường
```bash
# Copy file môi trường
cp env.example .env

# Chỉnh sửa file .env với các giá trị thực tế
nano .env
```

### 3. Build và chạy ứng dụng

#### Sử dụng script (khuyến nghị):
```bash
# Cấp quyền thực thi cho scripts
chmod +x scripts/*.sh

# Build ứng dụng
./scripts/build.sh

# Chạy ứng dụng
./scripts/run.sh

# Dừng ứng dụng
./scripts/stop.sh
```

#### Sử dụng Docker Compose trực tiếp:
```bash
# Build và chạy
docker-compose up -d --build

# Chỉ chạy (không build lại)
docker-compose up -d

# Dừng
docker-compose down
```

### 4. Truy cập ứng dụng

- **API**: http://localhost:5000
- **Swagger UI**: http://localhost:5000/swagger
- **SQL Server**: localhost:1433

## Cấu hình Production

### 1. Sử dụng docker-compose.prod.yml
```bash
docker-compose -f docker-compose.prod.yml up -d
```

### 2. Sử dụng Kubernetes
```bash
# Apply namespace
kubectl apply -f k8s/namespace.yaml

# Apply secrets và configmaps
kubectl apply -f k8s/secret.yaml
kubectl apply -f k8s/configmap.yaml

# Deploy SQL Server
kubectl apply -f k8s/sqlserver-deployment.yaml

# Deploy API
kubectl apply -f k8s/api-deployment.yaml
```

## Troubleshooting

### 1. Kiểm tra logs
```bash
# Xem logs của tất cả containers
docker-compose logs

# Xem logs của service cụ thể
docker-compose logs api
docker-compose logs sqlserver
```

### 2. Kiểm tra trạng thái containers
```bash
docker-compose ps
```

### 3. Restart service
```bash
docker-compose restart api
```

### 4. Xóa và tạo lại containers
```bash
docker-compose down -v
docker-compose up -d --build
```

## Cấu trúc Docker

```
FPTAlumniConnect_BE/
├── Dockerfile                 # Multi-stage build cho API
├── docker-compose.yml         # Development environment
├── docker-compose.prod.yml    # Production environment
├── .dockerignore             # Loại trừ files không cần thiết
├── env.example               # Template cho environment variables
├── scripts/                  # Scripts để build và run
│   ├── build.sh
│   ├── run.sh
│   └── stop.sh
└── k8s/                      # Kubernetes manifests
    ├── namespace.yaml
    ├── configmap.yaml
    ├── secret.yaml
    ├── sqlserver-deployment.yaml
    └── api-deployment.yaml
```

## Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| SA_PASSWORD | SQL Server password | YourStrong@Passw0rd |
| PERSPECTIVE_API_KEY | Perspective API key | - |
| PERSPECTIVE_API_ENDPOINT | Perspective API endpoint | https://commentmoderationservice.openai.azure.com/ |
| ASPNETCORE_ENVIRONMENT | .NET environment | Development |

## Security Notes

1. **Không commit file .env** vào repository
2. **Thay đổi mật khẩu mặc định** trong production
3. **Sử dụng secrets** trong Kubernetes
4. **Bật HTTPS** trong production
5. **Cấu hình firewall** cho database

## Performance Optimization

1. **Multi-stage builds** giảm kích thước image
2. **Health checks** để monitor ứng dụng
3. **Resource limits** trong Kubernetes
4. **Load balancing** với multiple replicas
5. **Caching** cho database connections 