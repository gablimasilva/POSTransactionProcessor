# POSTransactionProcessor

Projeto de processamento de transações com foco em arquitetura resiliente, idempotência e execução distribuída (cloud-native).

---

# 🚀 Como executar o projeto

## ✅ 1. Subir infraestrutura

Execute primeiro o docker-compose:

docker compose -f docker-compose.infra.yml up -d

Isso irá subir:

- PostgreSQL 🐘
- Redis 🧠
- Graylog 📊

---

## ✅ 2. Rodar a aplicação

### Opção 1 - VSCode / Visual Studio

- Abra a solution
- Execute o projeto Processor.WebApi

### Opção 2 - CLI

dotnet run --project Processor.WebApi

---

## ✅ 3. Testar via Postman

### Endpoint

POST http://localhost:5066/v1/pos/transactions/authorize

### Body

{
  "nsu": "123456",
  "amount": 100,
  "terminalId": "T1"
}

---

# 🔐 Autenticação (HMAC)

Utilize Pre-request Script no Postman:

```
  const secret = "super-secret";

// timestamp atual (segundos)
const timestamp = Math.floor(Date.now() / 1000);

// body da request
const body = pm.request.body.raw;

// payload = timestamp + body
const payload = timestamp + body;

// gerar hash
const signature = CryptoJS.HmacSHA256(payload, secret)
    .toString(CryptoJS.enc.Hex);

// adicionar headers automaticamente
pm.request.headers.upsert({
    key: "X-Timestamp",
    value: timestamp.toString()
});

pm.request.headers.upsert({
    key: "X-Signature",
    value: signature
});
```

---

# 🔁 Idempotência

A API garante idempotência na autorização utilizando:

- Chave composta: terminalId + nsu
- Redis como cache distribuído (TTL de 24 horas)
- Banco de dados com constraint única

Isso garante:

- Evita duplicação de transações
- Funciona corretamente em múltiplas instâncias (multi-pod)

---

# 🧠 Resiliência e Proteção (Polly)

A aplicação utiliza Polly para lidar com falhas externas e proteger o sistema.

---

## ✅ Timeouts

- Timeout configurado de 2 segundos
- Evita chamadas penduradas
- Falha rápida quando serviço externo demora

---

## ✅ Retry Controlado

- Máximo de 3 tentativas
- Backoff exponencial (tempo aumenta a cada tentativa)
- Jitter (variação aleatória)

Exemplo de comportamento:
100ms → 200ms → 400ms + variação aleatória

---

## ⚠️ Prevenção de Retry Storm

- Backoff exponencial evita sobrecarga
- Jitter evita sincronização entre instâncias
- Circuit breaker interrompe chamadas quando sistema está degradado

---

## ✅ Circuit Breaker

- Abre após 5 falhas consecutivas
- Permanece aberto por 30 segundos

### 🔴 Comportamento quando aberto

- Chamadas externas são interrompidas (fail fast)
- API retorna HTTP 503 (Service Unavailable)
- Evita sobrecarga do sistema

---

## ✅ Bulkhead (isolamento)

- Máximo de 50 chamadas concorrentes
- Fila de até 100 requisições

Isso impede que:

- threads sejam esgotadas
- CPU seja sobrecarregada
- dependência lenta derrube o sistema

---

## ✅ Policy aplicada

As estratégias são combinadas utilizando:

- Timeout
- Retry
- Circuit Breaker
- Bulkhead

---

# 📡 Status Codes

A API retorna:

- 200 OK → sucesso
- 204 NoContent → confirm/void executado
- 404 NotFound → transação não encontrada
- 422 UnprocessableEntity → erro de negócio
- 500 Internal Server Error → erro inesperado
- 503 Service Unavailable → circuit breaker aberto / falha externa

---

# 🔧 Middlewares

Middlewares utilizados:

- ExceptionMiddleware → tratamento global de erros
- CorrelationMiddleware → identificação de requisições
- LoggingMiddleware → logs estruturados

---

# 📊 Observabilidade

## ✅ Correlation ID

- Header: X-Correlation-Id
- Gerado automaticamente se não enviado
- Retornado na resposta
- Permite rastrear requisições ponta-a-ponta

---

## ✅ OpenTelemetry

- Tracing automático HTTP
- Métricas de latência
- Preparado para ferramentas como Grafana/Jaeger

---

## ✅ Graylog

- Logs centralizados via Serilog
- Transporte via GELF (UDP)
- Porta: 12201

Acesso:
http://localhost:9000

IMPORTANTE:
É necessário criar um input GELF UDP manualmente no Graylog.

---

# 🧠 Tecnologias utilizadas

- .NET 8
- ASP.NET Core
- Entity Framework Core
- PostgreSQL
- Redis
- Serilog
- Graylog
- Polly
- OpenTelemetry
- Docker / Docker Compose

---

# ⚙️ Arquitetura

- Clean Architecture
- DDD-lite
- Idempotência distribuída
- Pronto para múltiplas instâncias (cloud-native)

---

# ✅ Comportamento em produção

- Redis compartilhado (idempotência distribuída)
- Circuit breaker evita falhas em cascata
- Logs centralizados
- Preparado para Kubernetes / múltiplos pods

---

# 👨‍💻 Autor

Gabriel De Lima Da Silva