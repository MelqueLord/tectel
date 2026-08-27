# Assistência Técnica - Sistema de Gestão

Sistema web para gerenciamento de assistência técnica especializada em celulares e dispositivos eletrônicos.

## Arquitetura

```
src/
├── AssistenciaTecnica.Domain/       # Entidades e regras fundamentais
├── AssistenciaTecnica.Application/  # Casos de uso, DTOs, serviços
├── AssistenciaTecnica.Infrastructure/ # EF Core, MySQL, repositórios
└── AssistenciaTecnica.Web/          # ASP.NET Core MVC, Razor Views
tests/
└── AssistenciaTecnica.Tests/        # Testes unitários
```

## Tecnologias

- **Backend**: C# 13, .NET 10, ASP.NET Core MVC
- **Frontend**: Razor Views, Bootstrap 5, Bootstrap Icons
- **Banco de Dados**: SQLite (dev) / MySQL/MariaDB com Pomelo EF Core Provider
- **ORM**: Entity Framework Core 10
- **Testes**: xUnit, Moq

## Funcionalidades Implementadas

### Dashboard
- [x] Cards de indicadores (abertas, em manutenção, concluídas, etc.)
- [x] Ordens recentes com status e valores

### Clientes
- [x] Listagem com pesquisa e filtros
- [x] Cadastro com validações
- [x] Edição
- [x] Detalhes
- [x] Inativação/reativação

### Aparelhos
- [x] Listagem com pesquisa
- [x] Cadastro com seleção de cliente
- [x] Edição
- [x] Detalhes com histórico de ordens
- [x] Endpoint JSON para seleção dinâmica

### Ordens de Serviço
- [x] Listagem com pesquisa e filtros por status
- [x] Cadastro com seleção cliente/aparelho
- [x] Edição (diagnóstico, serviço, valores)
- [x] Detalhes completos
- [x] Alteração de status com validação de transições
- [x] Cálculo automático de valores
- [x] Geração automática de número

### Pagamentos
- [x] Registrar pagamento na OS
- [x] Múltiplas formas de pagamento (Dinheiro, PIX, Cartão, Transferência)
- [x] Histórico de pagamentos
- [x] Validação de saldo pendente
- [x] Validação de valor máximo
- [x] Acúmulo de valor pago

### Produtos
- [x] Listagem com pesquisa e filtros
- [x] Cadastro com validações
- [x] Edição
- [x] Detalhes
- [x] Código único
- [x] Alerta de estoque baixo

### Estoque
- [x] Listagem com filtro de estoque baixo
- [x] Entrada de estoque
- [x] Saída de estoque
- [x] Ajuste de estoque
- [x] Histórico de movimentações
- [x] Validação de estoque insuficiente

### Configurações
- [x] Dados da empresa (nome, CNPJ, contato, endereço)
- [x] Prazo padrão de garantia
- [x] Texto padrão da ordem de serviço
- [x] Criação automática se não existir

### Orçamentos
- [x] Listagem com pesquisa e filtros por status
- [x] Cadastro com itens (serviço/peça)
- [x] Edição
- [x] Detalhes
- [x] Alteração de status (Rascunho → Enviado → Aprovado/Rejeitado)
- [x] Conversão automática em OS
- [x] Seleção dinâmica de aparelhos por cliente

### Relatórios
- [x] Faturamento por período
- [x] Ordens por status
- [x] Pagamentos por período
- [x] Top clientes por gasto

## Regras de Negócio

- ValorTotal = ValorServico + ValorPecas - Desconto
- SaldoPendente = ValorTotal - ValorPago
- Transições de status controladas
- Número da OS único e gerado automaticamente
- Saída não pode deixar estoque negativo
- Todo movimento de estoque é registrado
- Pagamento não pode exceder saldo pendente
- Nome da assistência obrigatório

## Configuração Local

### Pré-requisitos
- .NET 10 SDK
- (Opcional) MySQL 8+ ou MariaDB 10.6+ ou Docker

### Opção 1: SQLite (Recomendado para desenvolvimento)
```bash
dotnet run --project src/AssistenciaTecnica.Web
```
O banco SQLite é criado automaticamente.

### Opção 2: MySQL com Docker
```bash
# Iniciar MySQL via Docker
docker-compose up -d

# Executar com MySQL
$env:ASPNETCORE_ENVIRONMENT="MySql"
dotnet run --project src/AssistenciaTecnica.Web
```

### Opção 3: MySQL Manual
```sql
CREATE DATABASE assistencia_tecnica CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

Crie `src/AssistenciaTecnica.Web/appsettings.MySql.json`:
```json
{
  "DatabaseProvider": "MySql",
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=assistencia_tecnica;User=root;Password=root;"
  }
}
```

Execute:
```bash
$env:ASPNETCORE_ENVIRONMENT="MySql"
dotnet run --project src/AssistenciaTecnica.Web
```

### Testes
```bash
dotnet test
```

## Testes (48)

| Módulo | Quantidade |
|--------|------------|
| Cliente | 10 |
| Aparelho | 4 |
| Ordem de Serviço | 10 |
| Produto/Estoque | 10 |
| Configurações | 6 |
| Pagamentos | 8 |

### Autenticação
- [x] Login com e-mail e senha
- [x] Registro de novos usuários
- [x] Proteção de todas as páginas com [Authorize]
- [x] Usuário admin seedado automaticamente
- [x] Exibição do usuário logado no topbar
- [x] Botão de logout
- [x] Página de acesso negado
- [x] Layout exclusivo para login/registro

### Logo da Empresa
- [x] Upload de imagem (JPG, PNG, GIF, SVG)
- [x] Exibição do logo no sidebar
- [x] Preview do logo atual nas configurações
- [x] Armazenamento em wwwroot/uploads

### Impressão de OS
- [x] Layout exclusivo para impressão (sem sidebar)
- [x] Cabeçalho com dados da empresa (nome, logo, CNPJ, telefone, endereço)
- [x] Dados do cliente e aparelho
- [x] Datas (entrada, previsão, conclusão, entrega)
- [x] Problema relatado, diagnóstico e serviço realizado
- [x] Valores (serviço, peças, desconto, total, pago, saldo)
- [x] Tabela de pagamentos
- [x] Garantia
- [x] Texto padrão da empresa (termos e condições)
- [x] Linhas de assinatura (cliente e assistência)
- [x] Botão Imprimir na página de detalhes

### MySQL
- [x] Configuração via appsettings.MySql.json
- [x] Suporte a Docker (docker-compose.yml)
- [x] Script de setup (scripts/setup-mysql.ps1)
- [x] Seleção de provider via configuração (DatabaseProvider)
- [x] Documentação completa

### Notificações
- [x] Sino de notificações no topbar com contador
- [x] Alertas de estoque baixo
- [x] Alertas de garantia vencendo (7 dias) e vencida
- [x] Alertas de entrega atrasada e para hoje
- [x] Dropdown com lista de notificações clicáveis
- [x] Ordenação por prioridade (danger > warning > info)
- [x] Links diretos para OS e Estoque

### Agendamentos
- [x] Calendário interativo (FullCalendar.js)
- [x] Visualizações: mês, semana, dia, lista
- [x] Tipos: Entrega, Revisão, Retirada, Manutenção, Outro
- [x] Status: Agendado, Confirmado, Concluído, Cancelado
- [x] Cores por tipo de agendamento
- [x] Criação rápida clicando no calendário
- [x] Vínculo com OS e Cliente
- [x] Lista de agendamentos do dia
- [x] Detalhes com ações de status

### Histórico do Cliente
- [x] Página completa com dados do cliente
- [x] Cards de resumo (total OS, abertas, concluídas, total gasto)
- [x] Lista de aparelhos do cliente
- [x] Tabela de ordens de serviço com status e valores
- [x] Links para detalhes de cada OS e aparelho
- [x] Botão "Histórico" na página de detalhes do cliente

### Técnicos
- [x] Listagem com pesquisa e filtros
- [x] Cadastro com validações
- [x] Edição
- [x] Detalhes
- [x] Inativação/reativação
- [x] Especialidade
- [x] Menu no sidebar

### Atribuição de Técnico na OS
- [x] Campo TécnicoId na entidade OrdemServico
- [x] Seleção de técnico ao criar OS
- [x] Seleção de técnico ao editar OS
- [x] Exibição do técnico na listagem de OS
- [x] Exibição do técnico nos detalhes da OS
- [x] Busca de OS por nome do técnico

## Próximas Tarefas

### NFS-e (Nota Fiscal de Serviço Eletrônica) - MEI
- [ ] Definir município do MEI (determina webservice)
- [ ] Obter token/credenciais do sistema de NFS-e
- [ ] Configuração: CNPJ, Inscrição Municipal, CNAE, código serviço LC 116, alíquota ISS
- [ ] Entidade NotaFiscalServico (número, série, valores, tomador, XML, status)
- [ ] Montagem do XML da NFS-e
- [ ] Integração com webservice (SOAP/REST)
- [ ] Consulta de protocolo
- [ ] Cancelamento de NFS-e
- [ ] Tela de configuração fiscal
- [ ] Botão "Emitir NFS-e" na OS concluída/entregue
- [ ] Tela de consulta/monitoramento de NFS-e
