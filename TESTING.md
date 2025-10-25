# ?? Testes R�pidos - Valida��o do Sistema

Este documento cont�m testes r�pidos para validar que todas as partes do sistema est�o funcionando corretamente.

## ? Checklist de Valida��o

### 1. ?? Docker Compose est� funcionando?

```powershell
# Windows
.\deploy.ps1 -Action deploy

# Linux/Mac
./deploy.sh deploy
```

**Resultado esperado:**
```
? Deploy conclu�do com sucesso!
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

### 2. ?? API est� respondendo?

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
- P�gina do Swagger UI carrega
- Endpoints vis�veis:
  - POST /api/autenticacao/login
  - GET /api/produtos
  - POST /api/produtos
  - etc.

---

### 3. ?? Autentica��o est� funcionando?

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

**? Se recebeu um token, autentica��o est� OK!**

---

### 4. ?? CRUD de Produtos est� funcionando?

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
    "localizacao": "Galp�o A - Prateleira 3"
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
"localizacao": "Galp�o A - Prateleira 3",
    "fotos": [],
    "dataCadastro": "2025-01-...",
    "dataAtualizacao": "2025-01-...",
    "statusSincronizacao": 1,
    "erroSincronizacao": null,
    "ativo": true
  }
]
```

#### Buscar produto espec�fico:

```bash
# Substitua {id} pelo ID retornado na cria��o
curl http://localhost:5000/api/produtos/{id}
```

#### Pesquisar produtos:

```bash
curl "http://localhost:5000/api/produtos/search?termo=adubo"
```

**Resultado esperado:**
- Array com produtos que cont�m "adubo" no nome, descri��o, etc.

**? Se todos os testes acima funcionaram, o CRUD est� OK!**

---

### 5. ?? Banco de Dados est� persistindo?

#### Teste de persist�ncia:

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
- O produto criado anteriormente ainda est� l�
- **? Persist�ncia est� funcionando!**

---

### 6. ?? Logs est�o sendo gerados?

#### Ver logs da API:

```bash
# Em tempo real
docker-compose logs -f api

# �ltimas 50 linhas
docker-compose logs --tail=50 api
```

**Resultado esperado:**
```
inventario-api | [INFO] Iniciando SSBJr.TecAgro.Inventario.Server
inventario-api | [INFO] Usu�rio admin criado com sucesso
inventario-api | [INFO] Servidor iniciado com sucesso
```

#### Ver logs do banco:

```bash
docker-compose logs db
```

**Resultado esperado:**
- SQL Server iniciado
- Database "InventarioDb" criado

**? Se v� logs estruturados, logging est� OK!**

---

### 7. ?? Volumes est�o persistindo dados?

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

**? Se os volumes existem, persist�ncia est� configurada!**

---

## ?? Testes Avan�ados

### Teste de Sincroniza��o Simulada

1. Criar produto com status pendente
2. Chamar endpoint de sincroniza��o (quando implementado)
3. Verificar mudan�a de status

### Teste de Autentica��o Inv�lida

```bash
curl -X POST "http://localhost:5000/api/autenticacao/login" \
  -H "Content-Type: application/json" \
  -d "{\"login\":\"admin\",\"senha\":\"senhaerrada\"}"
```

**Resultado esperado:**
- Status 401 Unauthorized

### Teste de Valida��o de Dados

```bash
curl -X POST "http://localhost:5000/api/produtos" \
  -H "Content-Type: application/json" \
  -d '{
    "nome": ""
  }'
```

**Resultado esperado:**
- Status 400 Bad Request (valida��o falhou)

---

## ?? Troubleshooting dos Testes

### Erro: "Connection refused"
**Causa:** API n�o est� rodando ou porta incorreta

**Solu��o:**
```bash
docker-compose ps  # Verificar status
docker-compose logs api  # Ver o que aconteceu
```

### Erro: "Database n�o existe"
**Causa:** Migrations n�o foram aplicadas

**Solu��o:**
- A API aplica migrations automaticamente no startup
- Verifique os logs: `docker-compose logs api`
- Se necess�rio, recrie: `docker-compose down -v && docker-compose up -d`

### Erro: "Invalid token"
**Causa:** Token expirado ou inv�lido

**Solu��o:**
- Fa�a login novamente para obter novo token
- Tokens expiram em 7 dias por padr�o

### Produtos n�o persistem ap�s restart
**Causa:** Volumes n�o configurados corretamente

**Solu��o:**
```bash
docker-compose down
docker volume ls  # Verificar se volumes existem
docker-compose up -d
```

---

## ? Checklist Final de Valida��o

- [ ] Docker Compose sobe sem erros
- [ ] API responde em http://localhost:5000
- [ ] Swagger UI carrega corretamente
- [ ] Login com admin/admin123 retorna token
- [ ] Cria��o de produto funciona
- [ ] Listagem de produtos funciona
- [ ] Busca de produtos funciona
- [ ] Dados persistem ap�s restart
- [ ] Logs s�o gerados corretamente
- [ ] Volumes est�o criados

**Se todos os itens acima est�o ?, seu sistema est� 100% funcional!**

---

## ?? M�tricas de Sucesso

### Performance Esperada:
- **Tempo de resposta da API:** < 200ms
- **Tempo de startup:** < 30 segundos
- **Tamanho da imagem Docker:** ~250MB
- **Uso de mem�ria:** ~512MB (API + DB)

### Monitoramento:
```bash
# Uso de recursos
docker stats

# Espa�o em disco
docker system df
```

---

## ?? Pr�ximos Passos

Se todos os testes passaram:

1. ? **Sistema validado e funcionando!**
2. ?? Implementar interface MAUI
3. ?? Adicionar captura de fotos
4. ?? Implementar sincroniza��o autom�tica
5. ?? Deploy em produ��o

---

## ?? Precisa de Ajuda?

Se algum teste falhou:
1. Verifique os logs: `docker-compose logs -f`
2. Consulte o `RUNNING_LOCALLY.md`
3. Abra uma issue no GitHub

**Desenvolvido com ?? pela equipe SSB Jr.**
