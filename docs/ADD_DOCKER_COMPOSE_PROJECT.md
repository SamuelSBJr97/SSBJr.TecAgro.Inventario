# ?? Como Adicionar Docker Compose Project à Solution

## Passos no Visual Studio 2022

### Método 1: Via Solution Explorer

1. **Abra a Solution**
   - Abra `SSBJr.TecAgro.Inventario.sln` no Visual Studio

2. **Adicione o Projeto Docker Compose**
   - No **Solution Explorer**, clique com botão direito na **Solution**
   - Selecione **Add** ? **Existing Project...**
   - Navegue até a raiz da solution
   - Selecione o arquivo **`docker-compose.dcproj`**
   - Clique em **Open**

3. **Configure como Startup**
   - Clique com botão direito no projeto **docker-compose**
   - Selecione **Set as Startup Project**

4. **Ou Configure Múltiplos Startups**
   - Clique com botão direito na **Solution**
   - Selecione **Properties**
   - Em **Common Properties** ? **Startup Project**
   - Selecione **Multiple startup projects**
   - Configure:
     ```
     docker-compose: Start
     SSBJr.TecAgro.Inventario.App: Start
     ```
   - Clique **OK**

5. **Pressione F5**
   - Docker Compose inicia automaticamente
   - App MAUI inicia em debug

### Método 2: Via Container Tools

1. **Habilitar Container Tools**
   - **Tools** ? **Options**
   - **Container Tools** ? **Docker Compose**
   - Marque:
     - ? **Automatically start Docker Compose**
     - ? **Docker Compose project debugging**

2. **Visual Studio detectará automaticamente**
   - Ao abrir a solution, VS detecta `docker-compose.dcproj`
   - Perguntará se deseja adicionar à solution
   - Clique **Yes**

## Estrutura da Solution Após Adicionar

```
Solution 'SSBJr.TecAgro.Inventario'
??? docker-compose (novo!)
??? src
?   ??? SSBJr.TecAgro.Inventario.App
?   ??? SSBJr.TecAgro.Inventario.Domain
?   ??? SSBJr.TecAgro.Inventario.Infrastructure
?   ??? SSBJr.TecAgro.Inventario.Server
??? tests
    ??? SSBJr.TecAgro.Inventario.Domain.Tests
    ??? SSBJr.TecAgro.Inventario.Infrastructure.Tests
  ??? SSBJr.TecAgro.Inventario.Server.Tests
```

## Benefícios

### Com Docker Compose Project

- ? **Gerenciar containers** diretamente do VS
- ? **Ver logs** na janela Output
- ? **Debug integrado** de containers
- ? **IntelliSense** em arquivos docker-compose
- ? **Controle visual** de containers

### Sem Docker Compose Project

- ? **Mais simples** - Apenas perfis de debug
- ? **Menos arquivos** na solution
- ? **Funciona igual** - Perfis já iniciam Docker

## Qual Método Escolher?

### Use Docker Compose Project se:
- Trabalha muito com containers
- Precisa debug avançado de containers
- Gosta de gerenciar tudo no Visual Studio
- Equipe usa Container Tools

### Use Apenas Perfis de Debug se:
- Quer simplicidade
- Docker é apenas dependência
- Não precisa debugar containers
- Prefere terminal para Docker

## Comando Alternativo

Se não quiser adicionar o projeto, pode usar apenas os perfis:

```
1. Selecione perfil "Android Emulator (Docker)"
2. Pressione F5
3. Docker Compose inicia automaticamente
```

Funciona perfeitamente sem adicionar `docker-compose.dcproj` à solution!

## Visualizar Containers

### Com Docker Compose Project
- **View** ? **Other Windows** ? **Containers**
- Veja todos os containers
- Clique direito para opções

### Sem Docker Compose Project
- Use Docker Desktop
- Ou terminal: `docker ps`

## Conclusão

**Ambos os métodos funcionam!**

- **Com projeto**: Mais integrado ao VS
- **Sem projeto**: Mais simples, funciona igual

Escolha o que preferir! ??
