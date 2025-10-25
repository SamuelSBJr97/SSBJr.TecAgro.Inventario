# ?? App MAUI + Docker - Guia de Configura��o

## ?? Vis�o Geral

O app MAUI agora est� configurado para se conectar automaticamente ao servidor hospedado no Docker, detectando a plataforma e configurando a URL correta automaticamente.

## ? Novidades Implementadas

### 1. Detec��o Autom�tica de Plataforma

O `PreferencesService` agora detecta automaticamente a plataforma e configura a URL correta:

| Plataforma | URL Padr�o | Descri��o |
|------------|-----------|-----------|
| **Android Emulator** | `http://10.0.2.2:5000` | IP especial para acessar localhost do host |
| **iOS Simulator** | `http://localhost:5000` | Pode acessar localhost diretamente |
| **Windows** | `http://localhost:5000` | Acesso direto ao localhost |
| **macOS** | `http://localhost:5000` | Acesso direto ao localhost |

### 2. P�gina de Configura��es

Nova p�gina **Configura��es** no app com:

- ? **Visualiza��o da plataforma atual**
- ? **Campo para editar URL do servidor**
- ? **Bot�o "Testar Conex�o"** - Verifica se o servidor est� acess�vel
- ? **Bot�o "Salvar Configura��es"** - Persiste a URL customizada
- ? **Bot�o "Restaurar Padr�o"** - Volta para a URL padr�o da plataforma
- ? **Dicas contextuais** - Orienta��es sobre URLs por plataforma

### 3. Arquivos Criados/Modificados

#### Novos Arquivos:
1. ? `ConfiguracoesViewModel.cs` - ViewModel da p�gina de configura��es
2. ? `ConfiguracoesPage.xaml` - Interface da p�gina
3. ? `ConfiguracoesPage.xaml.cs` - Code-behind

#### Modificados:
4. ? `PreferencesService.cs` - Detec��o autom�tica de plataforma
5. ? `MauiProgram.cs` - Registro de novos servi�os
6. ? `AppShell.xaml` - Adicionada aba "Configura��es"

## ?? Como Usar

### Primeira Execu��o

**1. Certifique-se que o Docker est� rodando:**

```powershell
# Verificar containers
docker ps

# Se n�o estiver rodando, inicie:
docker-compose -f docker-compose.dev.yml up -d
```

**2. Inicie a API:**

```powershell
cd src\SSBJr.TecAgro.Inventario.Server
dotnet run
```

**3. Execute o App MAUI no Visual Studio:**
- Pressione **F5**
- O app detectar� automaticamente a plataforma
- Configurar� a URL correta do Docker

### Configurar URL Manualmente

1. Abra o app
2. V� para a aba **"Configura��es"**
3. Altere a **URL do Servidor API** se necess�rio
4. Clique em **"Testar Conex�o"** para verificar
5. Clique em **"Salvar Configura��es"**

## ?? URLs por Cen�rio

### Android Emulator
```
http://10.0.2.2:5000
```
- ? URL configurada automaticamente
- ? Acessa Docker rodando no host Windows

### Android Device (F�sico)
```
http://192.168.1.XXX:5000
```
- ?? **Substitua XXX pelo IP da sua m�quina**
- ?? Dispositivo e PC devem estar na mesma rede WiFi

**Como descobrir seu IP:**

**Windows:**
```powershell
ipconfig
# Procure por "Endere�o IPv4"
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

## ?? Teste de Conex�o

A p�gina de configura��es inclui um bot�o **"Testar Conex�o"** que:

1. ? Faz uma requisi��o HTTP para o servidor
2. ? Verifica se a API est� respondendo
3. ? Exibe mensagem de sucesso ou erro
4. ? Timeout de 5 segundos

### Mensagens de Erro Comuns

| Erro | Causa | Solu��o |
|------|-------|---------|
| **"N�o foi poss�vel conectar"** | Servidor n�o est� rodando | Inicie Docker e API |
| **"Timeout ao conectar"** | URL incorreta ou firewall | Verifique URL e firewall |
| **"Status: 404"** | Endpoint n�o existe | Verifique se `/swagger/index.html` est� acess�vel |

## ?? Integrando com Docker

### Cen�rio 1: Desenvolvimento Local (Recomendado)

**Banco Docker + API Local:**

```powershell
# 1. Iniciar banco Docker
docker-compose -f docker-compose.dev.yml up -d

# 2. Executar API localmente
cd src\SSBJr.TecAgro.Inventario.Server
dotnet run

# 3. Executar app MAUI (F5 no VS)
# URL ser� configurada automaticamente
```

**Vantagens:**
- ? Hot reload da API
- ? Debug completo
- ? Menos recursos

### Cen�rio 2: Tudo no Docker

**Banco + API no Docker:**

```powershell
# 1. Iniciar todos os containers
docker-compose up -d --build

# 2. Executar app MAUI (F5 no VS)
# URL ser� configurada automaticamente
```

**Vantagens:**
- ? Ambiente mais pr�ximo da produ��o
- ? Isolamento completo

### Cen�rio 3: Debug com Docker (Visual Studio)

**Usando perfis de debug:**

1. Selecione perfil: **"Windows Machine (Docker)"** ou **"Android Emulator (Docker)"**
2. Pressione **F5**
3. Docker Compose inicia automaticamente
4. App se conecta � API no Docker

## ?? Fluxo de Configura��o no App

```
1. App inicia
   ?
2. PreferencesService detecta plataforma
   ?
3. Define URL padr�o (ex: http://10.0.2.2:5000 para Android)
   ?
4. Salva URL nas prefer�ncias
   ?
5. HttpClient usa essa URL para todas as requisi��es
```

## ?? Verificando Configura��o

### No App

1. Abra o app
2. V� para **Configura��es**
3. Veja **"Plataforma"** e **"URL do Servidor API"**
4. Clique em **"Testar Conex�o"**

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

### Problema: App n�o conecta ao servidor

**Solu��o 1: Verificar URL**
```csharp
// No app, v� para Configura��es
// Verifique se a URL est� correta para sua plataforma
```

**Solu��o 2: Verificar servidor**
```powershell
# Verificar se API est� rodando
curl http://localhost:5000/swagger/index.html

# Ou verificar Docker
docker ps
```

**Solu��o 3: Firewall**
```
- Windows: Permitir porta 5000 no Windows Firewall
- Antiv�rus: Adicionar exce��o para porta 5000
```

### Problema: Android Emulator n�o acessa 10.0.2.2

**Poss�veis causas:**
1. API n�o est� rodando no host
2. Porta 5000 bloqueada por firewall
3. Emulador n�o configurado corretamente

**Solu��o:**
```powershell
# 1. Verificar API rodando
netstat -ano | findstr :5000

# 2. Permitir porta no firewall
# Windows ? Firewall ? Regras de Entrada ? Nova Regra ? Porta 5000

# 3. No app, testar com IP expl�cito do PC
# Configura��es ? URL do Servidor ? http://SEU_IP:5000
```

### Problema: iOS Simulator n�o conecta

**Solu��o:**
```
1. Verificar se API est� em localhost:5000
2. iOS Simulator deve poder acessar localhost diretamente
3. Se n�o funcionar, use IP do Mac: http://SEU_IP:5000
```

## ?? Dicas

### 1. Durante Desenvolvimento

Use **API local + Banco Docker** para melhor experi�ncia:
```powershell
docker-compose -f docker-compose.dev.yml up -d
cd src\SSBJr.TecAgro.Inventario.Server
dotnet run
```

### 2. Dispositivos F�sicos

Para testar em dispositivos reais:
```
1. Descubra seu IP: ipconfig ou ifconfig
2. No app: Configura��es ? URL = http://SEU_IP:5000
3. PC e dispositivo na mesma rede WiFi
4. Firewall permitindo porta 5000
```

### 3. Debug Remoto

Se precisar debugar em dispositivo remoto:
```powershell
# Expor API para rede externa
dotnet run --urls "http://0.0.0.0:5000"

# No app, use o IP p�blico/externo
http://IP_PUBLICO:5000
```

## ?? Configura��o Padr�o por Plataforma

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

## ? Checklist de Verifica��o

- [ ] Docker Desktop rodando
- [ ] Container SQL Server iniciado (`docker ps`)
- [ ] API rodando (local ou Docker)
- [ ] Swagger acess�vel: http://localhost:5000/swagger
- [ ] App MAUI compilando sem erros
- [ ] URL configurada corretamente no app (Configura��es)
- [ ] Teste de conex�o bem-sucedido
- [ ] Login funcionando
- [ ] CRUD de produtos funcionando

## ?? Pr�ximos Passos

1. **Testar em todas as plataformas**
   - Android Emulator
 - iOS Simulator (se dispon�vel)
   - Windows Desktop

2. **Testar em dispositivo f�sico**
   - Configurar IP manualmente
   - Verificar conectividade

3. **Deploy com Docker completo**
   - Usar `docker-compose up -d --build`
   - Testar integra��o completa

## ?? Documenta��o Relacionada

- **[RESUMO_FINAL_PROJETO.md](../RESUMO_FINAL_PROJETO.md)** - Vis�o geral do projeto
- **[SOLUCAO_EXECUTAR_PROJETO.md](../SOLUCAO_EXECUTAR_PROJETO.md)** - Como executar
- **[VS_DOCKER_INTEGRATION.md](../VS_DOCKER_INTEGRATION.md)** - Integra��o VS + Docker
- **[DOCKER_GUIDE.md](../DOCKER_GUIDE.md)** - Guia Docker completo

---

**Implementado em**: 2025-01-24  
**Vers�o**: 1.0.0  
**Status**: ? Pronto para uso!

## ?? Resultado

O app MAUI agora:
- ? **Detecta automaticamente** a plataforma
- ? **Configura URL correta** do Docker
- ? **Permite customiza��o** via interface
- ? **Testa conex�o** antes de usar
- ? **Persiste configura��es** entre execu��es
- ? **Funciona offline** (SQLite local)
- ? **Sincroniza** quando online

**Pronto para desenvolvimento e produ��o!** ??
