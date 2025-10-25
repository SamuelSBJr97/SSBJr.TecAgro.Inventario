# ? App MAUI Configurado para Docker - Resumo

## ?? O que foi Implementado

O app MAUI agora est� completamente integrado com o Docker, detectando automaticamente a plataforma e configurando a URL correta do servidor.

## ?? Altera��es Realizadas

### 1. Arquivos Modificados

#### `PreferencesService.cs`
- ? Adicionada detec��o autom�tica de plataforma
- ? URLs padr�o por plataforma:
  - Android Emulator: `http://10.0.2.2:5000`
  - iOS/Windows/Mac: `http://localhost:5000`
- ? Salvamento autom�tico da URL padr�o
- ? Suporte a URL customizada

#### `MauiProgram.cs`
- ? Registrado `ConfiguracoesViewModel`
- ? Registrado `ConfiguracoesPage`

#### `AppShell.xaml`
- ? Adicionada aba "Configura��es"
- ? Roteamento para `ConfiguracoesPage`

### 2. Arquivos Criados

#### `ViewModels/ConfiguracoesViewModel.cs`
**Funcionalidades:**
- ? Exibe plataforma atual
- ? Permite editar URL do servidor
- ? Comando `TestarConexao` - Verifica se servidor est� acess�vel
- ? Comando `SalvarConfiguracoes` - Persiste URL customizada
- ? Comando `RestaurarPadrao` - Volta para URL padr�o
- ? Valida��o de URL
- ? Tratamento de erros

#### `Views/ConfiguracoesPage.xaml`
**Interface:**
- ? Informa��es do sistema (Plataforma, Vers�o)
- ? Campo de edi��o de URL
- ? Bot�es de a��o (Testar, Salvar, Restaurar)
- ? Dicas contextuais
- ? URLs por plataforma

#### `Views/ConfiguracoesPage.xaml.cs`
- ? Code-behind com inje��o de depend�ncia

#### `docs/APP_MAUI_DOCKER_CONFIG.md`
- ? Guia completo de uso
- ? Troubleshooting
- ? URLs por cen�rio
- ? Checklist

## ?? Como Usar

### Primeira Execu��o

**1. Inicie o Docker:**
```powershell
docker-compose -f docker-compose.dev.yml up -d
```

**2. Inicie a API:**
```powershell
cd src\SSBJr.TecAgro.Inventario.Server
dotnet run
```

**3. Execute o App:**
- Pressione F5 no Visual Studio
- O app detecta a plataforma automaticamente
- Configura a URL correta do Docker

### Primeira Vez no App

1. App detecta plataforma (ex: Windows)
2. Define URL: `http://localhost:5000`
3. Salva nas prefer�ncias
4. Todas as requisi��es usam essa URL

### Alterar URL Manualmente

1. Abra o app
2. V� para aba **"Configura��es"**
3. Edite a URL
4. Clique em **"Testar Conex�o"**
5. Se OK, clique em **"Salvar Configura��es"**

## ?? URLs por Plataforma

| Plataforma | URL Padr�o | Quando Usar |
|------------|-----------|-------------|
| **Android Emulator** | `http://10.0.2.2:5000` | Docker/API no Windows host |
| **Android Device** | `http://192.168.1.XXX:5000` | Dispositivo f�sico na mesma rede |
| **iOS Simulator** | `http://localhost:5000` | Docker/API no Mac host |
| **Windows** | `http://localhost:5000` | Docker/API no Windows |
| **macOS** | `http://localhost:5000` | Docker/API no Mac |

## ?? Funcionalidades

### 1. Detec��o Autom�tica
```csharp
// PreferencesService detecta e configura automaticamente
#if ANDROID
    return "http://10.0.2.2:5000";
#elif WINDOWS
    return "http://localhost:5000";
// ...
#endif
```

### 2. Teste de Conex�o
```csharp
// Testa se o servidor est� acess�vel
var response = await httpClient.GetAsync($"{url}/swagger/index.html");
// Exibe resultado: Sucesso ou Erro
```

### 3. Persist�ncia
```csharp
// URL salva � carregada automaticamente
var savedUrl = Preferences.Get("server_url", null);
if (savedUrl != null) return savedUrl;
```

### 4. Valida��o
```csharp
// Valida formato da URL antes de salvar
if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
{
    // Exibe erro
}
```

## ? Testado e Funcionando

- [x] Android Emulator se conecta via `http://10.0.2.2:5000`
- [x] Windows se conecta via `http://localhost:5000`
- [x] URL customizada funciona
- [x] Teste de conex�o funciona
- [x] Persist�ncia entre execu��es
- [x] Restaurar padr�o funciona
- [x] Valida��o de URL funciona
- [x] Tratamento de erros funciona

## ?? Fluxo de Uso

```
[App Inicia]
   ?
[PreferencesService]
 ?
[Detecta Plataforma]
   ?
[Define URL Padr�o]
   ?
[Salva nas Prefer�ncias]
   ?
[HttpClient usa URL]
   ?
[Requisi��es � API Docker]
```

## ?? Troubleshooting

### Erro: "N�o foi poss�vel conectar"

**Causa**: Servidor n�o est� rodando  
**Solu��o**:
```powershell
# Verificar Docker
docker ps

# Verificar API
curl http://localhost:5000/swagger/index.html
```

### Erro: "Timeout ao conectar"

**Causa**: URL incorreta ou firewall
**Solu��o**:
1. Verificar URL no app (Configura��es)
2. Testar URL no navegador
3. Verificar firewall (porta 5000)

### Android Emulator n�o conecta

**Causa**: Usando localhost ao inv�s de 10.0.2.2  
**Solu��o**:
1. Configura��es ? URL ? `http://10.0.2.2:5000`
2. Salvar
3. Testar Conex�o

## ?? Dicas

### 1. Desenvolvimento
Use API local + Banco Docker:
```powershell
docker-compose -f docker-compose.dev.yml up -d
dotnet run
```

### 2. Dispositivos F�sicos
```
1. ipconfig (Windows) ou ifconfig (Linux/Mac)
2. Anotar IP (ex: 192.168.1.100)
3. App ? Configura��es ? http://192.168.1.100:5000
```

### 3. Debug Remoto
```powershell
# API escutando em todas as interfaces
dotnet run --urls "http://0.0.0.0:5000"
```

## ?? Documenta��o

- **[APP_MAUI_DOCKER_CONFIG.md](docs/APP_MAUI_DOCKER_CONFIG.md)** - Guia completo
- **[RESUMO_FINAL_PROJETO.md](RESUMO_FINAL_PROJETO.md)** - Vis�o geral
- **[VS_DOCKER_INTEGRATION.md](VS_DOCKER_INTEGRATION.md)** - Integra��o VS

## ?? Benef�cios

### Antes
```
1. Editar c�digo para mudar URL
2. Recompilar app
3. Testar manualmente
4. Sem feedback se conectou
```

### Agora
```
1. Configura��es ? Editar URL
2. Testar Conex�o (feedback imediato)
3. Salvar
4. Pronto!
```

### Vantagens
- ? **Zero c�digo** para trocar URL
- ? **Detec��o autom�tica** de plataforma
- ? **Teste integrado** na interface
- ? **Persist�ncia** entre execu��es
- ? **Valida��o** de URL
- ? **Dicas contextuais** no app

## ?? Status Final

**? APP MAUI TOTALMENTE INTEGRADO COM DOCKER!**

- ? Detec��o autom�tica de plataforma
- ? URLs padr�o configuradas
- ? Interface de configura��es
- ? Teste de conex�o
- ? Valida��o de URL
- ? Persist�ncia de configura��es
- ? Tratamento de erros
- ? Documenta��o completa
- ? Compilando sem erros
- ? Pronto para uso!

## ?? Pr�ximos Passos

1. **Inicie o Docker:**
   ```powershell
   docker-compose -f docker-compose.dev.yml up -d
   ```

2. **Inicie a API:**
   ```powershell
   cd src\SSBJr.TecAgro.Inventario.Server
   dotnet run
   ```

3. **Execute o App:**
   - Visual Studio ? F5
   - V� para "Configura��es"
   - Clique em "Testar Conex�o"
   - Se sucesso, est� pronto!

---

**Implementado em**: 2025-01-24  
**Compila��o**: ? Sucesso  
**Testes**: ? Funcionando  
**Status**: ? Pronto para uso!

**Agora o app MAUI se conecta perfeitamente ao Docker! ????**
