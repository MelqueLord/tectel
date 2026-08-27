# Script para configurar MySQL no Windows
# Execute como Administrador

Write-Host "=== Configuração MySQL para Assistência Técnica ===" -ForegroundColor Cyan
Write-Host ""

# Verificar se Docker está instalado
$dockerInstalled = Get-Command docker -ErrorAction SilentlyContinue
if ($dockerInstalled) {
    Write-Host "Docker encontrado. Deseja usar Docker para o MySQL? (S/N)" -ForegroundColor Yellow
    $useDocker = Read-Host
    
    if ($useDocker -eq "S" -or $useDocker -eq "s") {
        Write-Host "Iniciando MySQL via Docker..." -ForegroundColor Green
        docker-compose up -d
        
        Write-Host "Aguardando MySQL inicializar..." -ForegroundColor Yellow
        Start-Sleep -Seconds 10
        
        Write-Host "MySQL iniciado com sucesso!" -ForegroundColor Green
        Write-Host "Connection String: Server=localhost;Port=3306;Database=assistencia_tecnica;User=assistencia;Password=assistencia123;" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "Para usar MySQL, execute:" -ForegroundColor Yellow
        Write-Host '  $env:ASPNETCORE_ENVIRONMENT="MySql"' -ForegroundColor White
        Write-Host '  dotnet run --project src/AssistenciaTecnica.Web' -ForegroundColor White
        exit
    }
}

# Configuração manual
Write-Host "Configuração Manual do MySQL" -ForegroundColor Yellow
Write-Host ""
Write-Host "Requisitos:" -ForegroundColor Cyan
Write-Host "  - MySQL 8.0+ ou MariaDB 10.6+" -ForegroundColor White
Write-Host "  - Banco de dados 'assistencia_tecnica' criado" -ForegroundColor White
Write-Host ""

$server = Read-Host "Server (default: localhost)"
if ([string]::IsNullOrEmpty($server)) { $server = "localhost" }

$port = Read-Host "Port (default: 3306)"
if ([string]::IsNullOrEmpty($port)) { $port = "3306" }

$database = Read-Host "Database (default: assistencia_tecnica)"
if ([string]::IsNullOrEmpty($database)) { $database = "assistencia_tecnica" }

$user = Read-Host "User (default: root)"
if ([string]::IsNullOrEmpty($user)) { $user = "root" }

$password = Read-Host "Password" -AsSecureString
$passwordPlain = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($password))

$connectionString = "Server=$server;Port=$port;Database=$database;User=$user;Password=$passwordPlain;"

Write-Host ""
Write-Host "Connection String gerada:" -ForegroundColor Green
Write-Host $connectionString -ForegroundColor Cyan
Write-Host ""
Write-Host "Para usar MySQL, crie o arquivo appsettings.MySql.json:" -ForegroundColor Yellow
Write-Host @"
{
  "DatabaseProvider": "MySql",
  "ConnectionStrings": {
    "DefaultConnection": "$connectionString"
  }
}
"@ -ForegroundColor White

Write-Host ""
Write-Host "Execute com:" -ForegroundColor Yellow
Write-Host '  $env:ASPNETCORE_ENVIRONMENT="MySql"' -ForegroundColor White
Write-Host '  dotnet run --project src/AssistenciaTecnica.Web' -ForegroundColor White
