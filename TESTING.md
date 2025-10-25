# ?? Testes Rápidos - Validação do Sistema

Este documento contém testes rápidos para validar que todas as partes do sistema estão funcionando corretamente.

## ? Checklist de Validação

### 1. ?? Docker Compose está funcionando?

```powershell
# Windows
.\deploy.ps1 -Action deploy

# Linux/Mac
./deploy.sh deploy
```

**Resultado esperado:**
```
? Deploy concluído com sucesso!
?? Acesse a API em: http://localhost:5000
```

**Verificar status:**
```bash
docker-compose ps
```

**Deve mostrar:**
```
NAME             STATUS
inventario-api   Up (healthy)
inventario-db    Up (healthy)
```

---

### 2. ?? API está respondendo?

#### Teste 1: Health Check
```bash
curl http://localhost:5000/api/produtos
```

**Resultado esperado:**
- Status 200 OK
- Array vazio `[]` (ainda sem produtos)

#### Teste 2: Swagger UI
Abra no navegador: **http://localhost:5000/swagger**

**Resultado esperado:**
- Página do Swagger UI carrega
- Endpoints visíveis:
  - POST /api/autenticacao/login
  - GET /api/produtos
  - POST /api/produtos
  - etc.

---

### 3. ?? Autenticação está funcionando?

#### Usando cURL:
```bash
curl -X POST "http://localhost:5000/api/autenticacao/login" \
  -H "Content-Type: application/json" \
  -d "{\"login\":\"admin\",\"senha\":\"admin123\"}"
```

**Resultado esperado:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

#### Usando PowerShell:
```powershell
$body = @{
    login = "admin"
    senha = "admin123"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/autenticacao/login" `
    -Method Post `
    -ContentType "application/json" `
    -Body $body
```

#### Usando Swagger UI:
1. Acesse http://localhost:5000/swagger
2. Clique em **POST /api/autenticacao/login**
3. Clique em **Try it out**
4. Cole:
```json
{
  "login": "admin",
  "senha": "admin123"
}
```
5. Clique em **Execute**
6. Copie o token retornado

**? Se recebeu um token, autenticação está OK!**

---

### 4. ?? CRUD de Produtos está funcionando?

#### Criar um produto:

```bash
curl -X POST "http://localhost:5000/api/produtos" \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "Adubo NPK 10-10-10",
    "descricao": "Fertilizante completo para todas as culturas",
    "codigoFiscal": "1234567890123",
    "sku": "ADU-NPK-001",
    "categoria": "Fertilizantes",
    "quantidadeEstoque": 100.5,
    "unidadeMedida": "KG",
    "valorAquisicao": 2.50,
    "valorRevenda": 3.80,
    "localizacao": "Galpão A - Prateleira 3"
  }'
```

**Resultado esperado:**
- Status 201 Created
- JSON com o produto criado incluindo um `id` (GUID)

#### Listar produtos:

```bash
curl http://localhost:5000/api/produtos
```

**Resultado esperado:**
```json
[
  {
    "id": "guid-aqui",
    "nome": "Adubo NPK 10-10-10",
    "descricao": "Fertilizante completo para todas as culturas",
    "codigoFiscal": "1234567890123",
    "sku": "ADU-NPK-001",
    "categoria": "Fertilizantes",
    "quantidadeEstoque": 100.5,
    "unidadeMedida": "KG",
    "valorAquisicao": 2.50,
    "valorRevenda": 3.80,
"localizacao": "Galpão A - Prateleira 3",
    "fotos": [],
    "dataCadastro": "2025-01-...",
    "dataAtualizacao": "2025-01-...",
    "statusSincronizacao": 1,
    "erroSincronizacao": null,
    "ativo": true
  }
]
```

#### Buscar produto específico:

```bash
# Substitua {id} pelo ID retornado na criação
curl http://localhost:5000/api/produtos/{id}
```

#### Pesquisar produtos:

```bash
curl "http://localhost:5000/api/produtos/search?termo=adubo"
```

**Resultado esperado:**
- Array com produtos que contêm "adubo" no nome, descrição, etc.

**? Se todos os testes acima funcionaram, o CRUD está OK!**

---

### 5. ?? Banco de Dados está persistindo?

#### Teste de persistência:

1. Crie um produto (teste anterior)
2. Pare os containers:
```bash
docker-compose down
```
3. Inicie novamente:
```bash
docker-compose up -d
```
4. Liste os produtos:
```bash
curl http://localhost:5000/api/produtos
```

**Resultado esperado:**
- O produto criado anteriormente ainda está lá
- **? Persistência está funcionando!**

---

### 6. ?? Logs estão sendo gerados?

#### Ver logs da API:

```bash
# Em tempo real
docker-compose logs -f api

# Últimas 50 linhas
docker-compose logs --tail=50 api
```

**Resultado esperado:**
```
inventario-api | [INFO] Iniciando SSBJr.TecAgro.Inventario.Server
inventario-api | [INFO] Usuário admin criado com sucesso
inventario-api | [INFO] Servidor iniciado com sucesso
```

#### Ver logs do banco:

```bash
docker-compose logs db
```

**Resultado esperado:**
- SQL Server iniciado
- Database "InventarioDb" criado

**? Se vê logs estruturados, logging está OK!**

---

### 7. ?? Volumes estão persistindo dados?

#### Verificar volumes criados:

```bash
docker volume ls
```

**Resultado esperado:**
```
VOLUME NAME
ssbjr.tecagro.inventario_sql-data
ssbjr.tecagro.inventario_api-storage
ssbjr.tecagro.inventario_api-logs
```

#### Inspecionar volume:

```bash
docker volume inspect ssbjr.tecagro.inventario_sql-data
```

**? Se os volumes existem, persistência está configurada!**

---

## ?? Testes Avançados

### Teste de Sincronização Simulada

1. Criar produto com status pendente
2. Chamar endpoint de sincronização (quando implementado)
3. Verificar mudança de status

### Teste de Autenticação Inválida

```bash
curl -X POST "http://localhost:5000/api/autenticacao/login" \
  -H "Content-Type: application/json" \
  -d "{\"login\":\"admin\",\"senha\":\"senhaerrada\"}"
```

**Resultado esperado:**
- Status 401 Unauthorized

### Teste de Validação de Dados

```bash
curl -X POST "http://localhost:5000/api/produtos" \
  -H "Content-Type: application/json" \
  -d '{
    "nome": ""
  }'
```

**Resultado esperado:**
- Status 400 Bad Request (validação falhou)

---

## ?? Troubleshooting dos Testes

### Erro: "Connection refused"
**Causa:** API não está rodando ou porta incorreta

**Solução:**
```bash
docker-compose ps  # Verificar status
docker-compose logs api  # Ver o que aconteceu
```

### Erro: "Database não existe"
**Causa:** Migrations não foram aplicadas

**Solução:**
- A API aplica migrations automaticamente no startup
- Verifique os logs: `docker-compose logs api`
- Se necessário, recrie: `docker-compose down -v && docker-compose up -d`

### Erro: "Invalid token"
**Causa:** Token expirado ou inválido

**Solução:**
- Faça login novamente para obter novo token
- Tokens expiram em 7 dias por padrão

### Produtos não persistem após restart
**Causa:** Volumes não configurados corretamente

**Solução:**
```bash
docker-compose down
docker volume ls  # Verificar se volumes existem
docker-compose up -d
```

---

## ? Checklist Final de Validação

- [ ] Docker Compose sobe sem erros
- [ ] API responde em http://localhost:5000
- [ ] Swagger UI carrega corretamente
- [ ] Login com admin/admin123 retorna token
- [ ] Criação de produto funciona
- [ ] Listagem de produtos funciona
- [ ] Busca de produtos funciona
- [ ] Dados persistem após restart
- [ ] Logs são gerados corretamente
- [ ] Volumes estão criados

**Se todos os itens acima estão ?, seu sistema está 100% funcional!**

---

## ?? Métricas de Sucesso

### Performance Esperada:
- **Tempo de resposta da API:** < 200ms
- **Tempo de startup:** < 30 segundos
- **Tamanho da imagem Docker:** ~250MB
- **Uso de memória:** ~512MB (API + DB)

### Monitoramento:
```bash
# Uso de recursos
docker stats

# Espaço em disco
docker system df
```

---

## ?? Próximos Passos

Se todos os testes passaram:

1. ? **Sistema validado e funcionando!**
2. ?? Implementar interface MAUI
3. ?? Adicionar captura de fotos
4. ?? Implementar sincronização automática
5. ?? Deploy em produção

---

## ?? Precisa de Ajuda?

Se algum teste falhou:
1. Verifique os logs: `docker-compose logs -f`
2. Consulte o `RUNNING_LOCALLY.md`
3. Abra uma issue no GitHub

**Desenvolvido com ?? pela equipe SSB Jr.**
