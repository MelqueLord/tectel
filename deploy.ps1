# ===========================================
# TECTEL - Deploy Script for Hostinger (Windows)
# ===========================================

$ErrorActionPreference = "Stop"

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "   TECTEL - Deploy para Produção" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan

# Check if .env file exists
if (-not (Test-Path .env)) {
    Write-Host "❌ Arquivo .env não encontrado!" -ForegroundColor Red
    Write-Host "📝 Copie o .env.example para .env e configure as variáveis:" -ForegroundColor Yellow
    Write-Host "   cp .env.example .env" -ForegroundColor White
    Write-Host "   notepad .env" -ForegroundColor White
    exit 1
}

# Load environment variables
Get-Content .env | ForEach-Object {
    if ($_ -match '^([^#][^=]+)=(.*)$') {
        [Environment]::SetEnvironmentVariable($matches[1].Trim(), $matches[2].Trim(), "Process")
    }
}

# Validate required variables
$adminPassword = [Environment]::GetEnvironmentVariable("ADMIN_PASSWORD", "Process")
$mysqlRootPassword = [Environment]::GetEnvironmentVariable("MYSQL_ROOT_PASSWORD", "Process")
$mysqlPassword = [Environment]::GetEnvironmentVariable("MYSQL_PASSWORD", "Process")

if ($adminPassword -eq "CHANGE_THIS_STRONG_PASSWORD" -or [string]::IsNullOrEmpty($adminPassword)) {
    Write-Host "❌ ADMIN_PASSWORD não configurado ou ainda está com valor padrão!" -ForegroundColor Red
    Write-Host "📝 Edite o .env e defina uma senha segura." -ForegroundColor Yellow
    exit 1
}

if ($mysqlRootPassword -eq "CHANGE_THIS_ROOT_PASSWORD" -or [string]::IsNullOrEmpty($mysqlRootPassword)) {
    Write-Host "❌ MYSQL_ROOT_PASSWORD não configurado ou ainda está com valor padrão!" -ForegroundColor Red
    Write-Host "📝 Edite o .env e defina uma senha segura." -ForegroundColor Yellow
    exit 1
}

if ($mysqlPassword -eq "CHANGE_THIS_DB_PASSWORD" -or [string]::IsNullOrEmpty($mysqlPassword)) {
    Write-Host "❌ MYSQL_PASSWORD não configurado ou ainda está com valor padrão!" -ForegroundColor Red
    Write-Host "📝 Edite o .env e defina uma senha segura." -ForegroundColor Yellow
    exit 1
}

Write-Host "✅ Variáveis de ambiente validadas" -ForegroundColor Green

# Create necessary directories
Write-Host "📁 Criando diretórios..." -ForegroundColor Yellow
New-Item -ItemType Directory -Force -Path nginx/ssl | Out-Null
New-Item -ItemType Directory -Force -Path logs | Out-Null

# Stop existing containers
Write-Host "🛑 Parando containers existentes..." -ForegroundColor Yellow
docker-compose -f docker-compose.prod.yml down 2>$null

# Build and start containers
Write-Host "🔨 Construindo imagens..." -ForegroundColor Yellow
docker-compose -f docker-compose.prod.yml build --no-cache

Write-Host "🚀 Iniciando serviços..." -ForegroundColor Yellow
docker-compose -f docker-compose.prod.yml up -d

# Wait for MySQL to be ready
Write-Host "⏳ Aguardando MySQL inicializar..." -ForegroundColor Yellow
Start-Sleep -Seconds 30

# Check if services are running
Write-Host "🔍 Verificando serviços..." -ForegroundColor Yellow
docker-compose -f docker-compose.prod.yml ps

Write-Host ""
Write-Host "==========================================" -ForegroundColor Green
Write-Host "   ✅ Deploy concluído com sucesso!" -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Green
Write-Host ""
Write-Host "📊 Status dos serviços:" -ForegroundColor Cyan
$port = [Environment]::GetEnvironmentVariable("APP_PORT", "Process")
if ([string]::IsNullOrEmpty($port)) { $port = "8080" }
Write-Host "   - Aplicação: http://localhost:$port" -ForegroundColor White
Write-Host "   - MySQL: localhost:3306" -ForegroundColor White
Write-Host "   - Nginx: http://localhost:80" -ForegroundColor White
Write-Host ""
Write-Host "🔑 Credenciais de acesso:" -ForegroundColor Cyan
$email = [Environment]::GetEnvironmentVariable("ADMIN_EMAIL", "Process")
if ([string]::IsNullOrEmpty($email)) { $email = "admin@tectel.com.br" }
Write-Host "   - Email: $email" -ForegroundColor White
Write-Host "   - Senha: (a que você definiu no .env)" -ForegroundColor White
Write-Host ""
Write-Host "📋 Comandos úteis:" -ForegroundColor Cyan
Write-Host "   - Ver logs: docker-compose -f docker-compose.prod.yml logs -f" -ForegroundColor White
Write-Host "   - Parar: docker-compose -f docker-compose.prod.yml down" -ForegroundColor White
Write-Host "   - Reiniciar: docker-compose -f docker-compose.prod.yml restart" -ForegroundColor White
Write-Host ""
