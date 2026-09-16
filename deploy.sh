#!/bin/bash
# ===========================================
# TECTEL - Deploy Script for Hostinger
# ===========================================

set -e

echo "=========================================="
echo "   TECTEL - Deploy para Produção"
echo "=========================================="

# Check if .env file exists
if [ ! -f .env ]; then
    echo "❌ Arquivo .env não encontrado!"
    echo "📝 Copie o .env.example para .env e configure as variáveis:"
    echo "   cp .env.example .env"
    echo "   nano .env"
    exit 1
fi

# Load environment variables
source .env

# Validate required variables
if [ "$ADMIN_PASSWORD" = "CHANGE_THIS_STRONG_PASSWORD" ] || [ -z "$ADMIN_PASSWORD" ]; then
    echo "❌ ADMIN_PASSWORD não configurado ou ainda está com valor padrão!"
    echo "📝 Edite o .env e defina uma senha segura."
    exit 1
fi

if [ "$MYSQL_ROOT_PASSWORD" = "CHANGE_THIS_ROOT_PASSWORD" ] || [ -z "$MYSQL_ROOT_PASSWORD" ]; then
    echo "❌ MYSQL_ROOT_PASSWORD não configurado ou ainda está com valor padrão!"
    echo "📝 Edite o .env e defina uma senha segura."
    exit 1
fi

if [ "$MYSQL_PASSWORD" = "CHANGE_THIS_DB_PASSWORD" ] || [ -z "$MYSQL_PASSWORD" ]; then
    echo "❌ MYSQL_PASSWORD não configurado ou ainda está com valor padrão!"
    echo "📝 Edite o .env e defina uma senha segura."
    exit 1
fi

echo "✅ Variáveis de ambiente validadas"

# Create necessary directories
echo "📁 Criando diretórios..."
mkdir -p nginx/ssl
mkdir -p logs

# Stop existing containers
echo "🛑 Parando containers existentes..."
docker-compose -f docker-compose.prod.yml down 2>/dev/null || true

# Build and start containers
echo "🔨 Construindo imagens..."
docker-compose -f docker-compose.prod.yml build --no-cache

echo "🚀 Iniciando serviços..."
docker-compose -f docker-compose.prod.yml up -d

# Wait for MySQL to be ready
echo "⏳ Aguardando MySQL inicializar..."
sleep 30

# Check if services are running
echo "🔍 Verificando serviços..."
docker-compose -f docker-compose.prod.yml ps

echo ""
echo "=========================================="
echo "   ✅ Deploy concluído com sucesso!"
echo "=========================================="
echo ""
echo "📊 Status dos serviços:"
echo "   - Aplicação: http://localhost:${APP_PORT:-8080}"
echo "   - MySQL: localhost:${MYSQL_PORT:-3306}"
echo "   - Nginx: http://localhost:${HTTP_PORT:-80}"
echo ""
echo "🔑 Credenciais de acesso:"
echo "   - Email: ${ADMIN_EMAIL:-admin@tectel.com.br}"
echo "   - Senha: (a que você definiu no .env)"
echo ""
echo "📋 Comandos úteis:"
echo "   - Ver logs: docker-compose -f docker-compose.prod.yml logs -f"
echo "   - Parar: docker-compose -f docker-compose.prod.yml down"
echo "   - Reiniciar: docker-compose -f docker-compose.prod.yml restart"
echo ""
