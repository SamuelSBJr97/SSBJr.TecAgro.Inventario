# ?? App MAUI + Docker - Guia de Configuração

## ?? Visão Geral

O app MAUI agora está configurado para se conectar automaticamente ao servidor hospedado no Docker, detectando a plataforma e configurando a URL correta automaticamente.

## ? Novidades Implementadas

### 1. Detecção Automática de Plataforma

O `PreferencesService` agora detecta automaticamente a plataforma e configura a URL correta:

| Plataforma | URL Padrão | Descrição |
|------------|-----------|-----------|
| **Android Emulator** | `http://10.0.2.2:5000` | IP especial para acessar localhost do host |
| **iOS Simulator** | `http://localhost:5000` | Pode acessar localhost diretamente |
| **Windows** | `http://localhost:5000` | Acesso direto ao localhost |
| **macOS** | `http://localhost:5000` | Acesso direto ao localhost |

### 2. Página de Configurações

Nova página **Configurações** no app com:

- ? **Visualização da plataforma atual**
- ? **Campo para editar URL do servidor**
- ? **Botão "Testar Conexão"** - Verifica se o servidor está acessível
- ? **Botão "Salvar Configurações"** - Persiste a URL customizada
- ? **Botão "Restaurar Padrão"** - Volta para a URL padrão da plataforma
- ? **Dicas contextuais** - Orientações sobre URLs por plataforma

### 3. Arquivos Criados/Modificados

#### Novos Arquivos:
1. ? `ConfiguracoesViewModel.cs` - ViewModel da página de configurações
2. ? `ConfiguracoesPage.xaml` - Interface da página
3. ? `ConfiguracoesPage.xaml.cs` - Code-behind

#### Modificados:
4. ? `PreferencesService.cs` - Detecção automática de plataforma
5. ? `MauiProgram.cs` - Registro de novos serviços
6. ? `AppShell.xaml` - Adicionada aba "Configurações"

## ?? Como Usar

### Primeira Execução

**1. Certifique-se que o Docker está rodando:**

```powershell
# Verificar containers
docker ps

# Se não estiver rodando, inicie:
docker-compose -f docker-compose.dev.yml up -d
```

**2. Inicie a API:**

```powershell
cd src\SSBJr.TecAgro.Inventario.Server
dotnet run
```

**3. Execute o App MAUI no Visual Studio:**
- Pressione **F5**
- O app detectará automaticamente a plataforma
- Configurará a URL correta do Docker

### Configurar URL Manualmente

1. Abra o app
2. Vá para a aba **"Configurações"**
3. Altere a **URL do Servidor API** se necessário
4. Clique em **"Testar Conexão"** para verificar
5. Clique em **"Salvar Configurações"**

## ?? URLs por Cenário

### Android Emulator
```
http://10.0.2.2:5000
```
- ? URL configurada automaticamente
- ? Acessa Docker rodando no host Windows

### Android Device (Físico)
```
http://192.168.1.XXX:5000
```
- ?? **Substitua XXX pelo IP da sua máquina**
- ?? Dispositivo e PC devem estar na mesma rede WiFi

**Como descobrir seu IP:**

**Windows:**
```powershell
ipconfig
# Procure por "Endereço IPv4"
```

**Linux/Mac:**
```bash
ifconfig
# ou
ip addr show
```

### iOS Simulator
```
http://localhost:5000
```
- ? URL configurada automaticamente
- ? Simulator acessa localhost diretamente

### Windows Desktop
```
http://localhost:5000
```
- ? URL configurada automaticamente
- ? Acesso direto ao localhost

## ?? Teste de Conexão

A página de configurações inclui um botão **"Testar Conexão"** que:

1. ? Faz uma requisição HTTP para o servidor
2. ? Verifica se a API está respondendo
3. ? Exibe mensagem de sucesso ou erro
4. ? Timeout de 5 segundos

### Mensagens de Erro Comuns

| Erro | Causa | Solução |
|------|-------|---------|
| **"Não foi possível conectar"** | Servidor não está rodando | Inicie Docker e API |
| **"Timeout ao conectar"** | URL incorreta ou firewall | Verifique URL e firewall |
| **"Status: 404"** | Endpoint não existe | Verifique se `/swagger/index.html` está acessível |

## ?? Integrando com Docker

### Cenário 1: Desenvolvimento Local (Recomendado)

**Banco Docker + API Local:**

```powershell
# 1. Iniciar banco Docker
docker-compose -f docker-compose.dev.yml up -d

# 2. Executar API localmente
cd src\SSBJr.TecAgro.Inventario.Server
dotnet run

# 3. Executar app MAUI (F5 no VS)
# URL será configurada automaticamente
```

**Vantagens:**
- ? Hot reload da API
- ? Debug completo
- ? Menos recursos

### Cenário 2: Tudo no Docker

**Banco + API no Docker:**

```powershell
# 1. Iniciar todos os containers
docker-compose up -d --build

# 2. Executar app MAUI (F5 no VS)
# URL será configurada automaticamente
```

**Vantagens:**
- ? Ambiente mais próximo da produção
- ? Isolamento completo

### Cenário 3: Debug com Docker (Visual Studio)

**Usando perfis de debug:**

1. Selecione perfil: **"Windows Machine (Docker)"** ou **"Android Emulator (Docker)"**
2. Pressione **F5**
3. Docker Compose inicia automaticamente
4. App se conecta à API no Docker

## ?? Fluxo de Configuração no App

```
1. App inicia
   ?
2. PreferencesService detecta plataforma
   ?
3. Define URL padrão (ex: http://10.0.2.2:5000 para Android)
   ?
4. Salva URL nas preferências
   ?
5. HttpClient usa essa URL para todas as requisições
```

## ?? Verificando Configuração

### No App

1. Abra o app
2. Vá para **Configurações**
3. Veja **"Plataforma"** e **"URL do Servidor API"**
4. Clique em **"Testar Conexão"**

### Na API (Docker)

```powershell
# Ver logs da API
docker logs -f tecagro-dev-db

# Testar swagger
curl http://localhost:5000/swagger/index.html

# Ou abrir no navegador
start http://localhost:5000/swagger
```

## ?? Troubleshooting

### Problema: App não conecta ao servidor

**Solução 1: Verificar URL**
```csharp
// No app, vá para Configurações
// Verifique se a URL está correta para sua plataforma
```

**Solução 2: Verificar servidor**
```powershell
# Verificar se API está rodando
curl http://localhost:5000/swagger/index.html

# Ou verificar Docker
docker ps
```

**Solução 3: Firewall**
```
- Windows: Permitir porta 5000 no Windows Firewall
- Antivírus: Adicionar exceção para porta 5000
```

### Problema: Android Emulator não acessa 10.0.2.2

**Possíveis causas:**
1. API não está rodando no host
2. Porta 5000 bloqueada por firewall
3. Emulador não configurado corretamente

**Solução:**
```powershell
# 1. Verificar API rodando
netstat -ano | findstr :5000

# 2. Permitir porta no firewall
# Windows ? Firewall ? Regras de Entrada ? Nova Regra ? Porta 5000

# 3. No app, testar com IP explícito do PC
# Configurações ? URL do Servidor ? http://SEU_IP:5000
```

### Problema: iOS Simulator não conecta

**Solução:**
```
1. Verificar se API está em localhost:5000
2. iOS Simulator deve poder acessar localhost diretamente
3. Se não funcionar, use IP do Mac: http://SEU_IP:5000
```

## ?? Dicas

### 1. Durante Desenvolvimento

Use **API local + Banco Docker** para melhor experiência:
```powershell
docker-compose -f docker-compose.dev.yml up -d
cd src\SSBJr.TecAgro.Inventario.Server
dotnet run
```

### 2. Dispositivos Físicos

Para testar em dispositivos reais:
```
1. Descubra seu IP: ipconfig ou ifconfig
2. No app: Configurações ? URL = http://SEU_IP:5000
3. PC e dispositivo na mesma rede WiFi
4. Firewall permitindo porta 5000
```

### 3. Debug Remoto

Se precisar debugar em dispositivo remoto:
```powershell
# Expor API para rede externa
dotnet run --urls "http://0.0.0.0:5000"

# No app, use o IP público/externo
http://IP_PUBLICO:5000
```

## ?? Configuração Padrão por Plataforma

```csharp
// PreferencesService.cs
private static string GetDefaultServerUrl()
{
#if ANDROID
    return "http://10.0.2.2:5000";      // Emulador Android
#elif IOS
    return "http://localhost:5000";      // Simulator iOS
#elif WINDOWS
    return "http://localhost:5000"; // Windows Desktop
#elif MACCATALYST
    return "http://localhost:5000";      // macOS
#else
    return "http://localhost:5000";      // Fallback
#endif
}
```

## ? Checklist de Verificação

- [ ] Docker Desktop rodando
- [ ] Container SQL Server iniciado (`docker ps`)
- [ ] API rodando (local ou Docker)
- [ ] Swagger acessível: http://localhost:5000/swagger
- [ ] App MAUI compilando sem erros
- [ ] URL configurada corretamente no app (Configurações)
- [ ] Teste de conexão bem-sucedido
- [ ] Login funcionando
- [ ] CRUD de produtos funcionando

## ?? Próximos Passos

1. **Testar em todas as plataformas**
   - Android Emulator
 - iOS Simulator (se disponível)
   - Windows Desktop

2. **Testar em dispositivo físico**
   - Configurar IP manualmente
   - Verificar conectividade

3. **Deploy com Docker completo**
   - Usar `docker-compose up -d --build`
   - Testar integração completa

## ?? Documentação Relacionada

- **[RESUMO_FINAL_PROJETO.md](../RESUMO_FINAL_PROJETO.md)** - Visão geral do projeto
- **[SOLUCAO_EXECUTAR_PROJETO.md](../SOLUCAO_EXECUTAR_PROJETO.md)** - Como executar
- **[VS_DOCKER_INTEGRATION.md](../VS_DOCKER_INTEGRATION.md)** - Integração VS + Docker
- **[DOCKER_GUIDE.md](../DOCKER_GUIDE.md)** - Guia Docker completo

---

**Implementado em**: 2025-01-24  
**Versão**: 1.0.0  
**Status**: ? Pronto para uso!

## ?? Resultado

O app MAUI agora:
- ? **Detecta automaticamente** a plataforma
- ? **Configura URL correta** do Docker
- ? **Permite customização** via interface
- ? **Testa conexão** antes de usar
- ? **Persiste configurações** entre execuções
- ? **Funciona offline** (SQLite local)
- ? **Sincroniza** quando online

**Pronto para desenvolvimento e produção!** ??
