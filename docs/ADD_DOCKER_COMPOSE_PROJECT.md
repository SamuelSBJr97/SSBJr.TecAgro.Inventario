# ?? Como Adicionar Docker Compose Project � Solution

## Passos no Visual Studio 2022

### M�todo 1: Via Solution Explorer

1. **Abra a Solution**
   - Abra `SSBJr.TecAgro.Inventario.sln` no Visual Studio

2. **Adicione o Projeto Docker Compose**
   - No **Solution Explorer**, clique com bot�o direito na **Solution**
   - Selecione **Add** ? **Existing Project...**
   - Navegue at� a raiz da solution
   - Selecione o arquivo **`docker-compose.dcproj`**
   - Clique em **Open**

3. **Configure como Startup**
   - Clique com bot�o direito no projeto **docker-compose**
   - Selecione **Set as Startup Project**

4. **Ou Configure M�ltiplos Startups**
   - Clique com bot�o direito na **Solution**
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

### M�todo 2: Via Container Tools

1. **Habilitar Container Tools**
   - **Tools** ? **Options**
   - **Container Tools** ? **Docker Compose**
   - Marque:
     - ? **Automatically start Docker Compose**
     - ? **Docker Compose project debugging**

2. **Visual Studio detectar� automaticamente**
   - Ao abrir a solution, VS detecta `docker-compose.dcproj`
   - Perguntar� se deseja adicionar � solution
   - Clique **Yes**

## Estrutura da Solution Ap�s Adicionar

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

## Benef�cios

### Com Docker Compose Project

- ? **Gerenciar containers** diretamente do VS
- ? **Ver logs** na janela Output
- ? **Debug integrado** de containers
- ? **IntelliSense** em arquivos docker-compose
- ? **Controle visual** de containers

### Sem Docker Compose Project

- ? **Mais simples** - Apenas perfis de debug
- ? **Menos arquivos** na solution
- ? **Funciona igual** - Perfis j� iniciam Docker

## Qual M�todo Escolher?

### Use Docker Compose Project se:
- Trabalha muito com containers
- Precisa debug avan�ado de containers
- Gosta de gerenciar tudo no Visual Studio
- Equipe usa Container Tools

### Use Apenas Perfis de Debug se:
- Quer simplicidade
- Docker � apenas depend�ncia
- N�o precisa debugar containers
- Prefere terminal para Docker

## Comando Alternativo

Se n�o quiser adicionar o projeto, pode usar apenas os perfis:

```
1. Selecione perfil "Android Emulator (Docker)"
2. Pressione F5
3. Docker Compose inicia automaticamente
```

Funciona perfeitamente sem adicionar `docker-compose.dcproj` � solution!

## Visualizar Containers

### Com Docker Compose Project
- **View** ? **Other Windows** ? **Containers**
- Veja todos os containers
- Clique direito para op��es

### Sem Docker Compose Project
- Use Docker Desktop
- Ou terminal: `docker ps`

## Conclus�o

**Ambos os m�todos funcionam!**

- **Com projeto**: Mais integrado ao VS
- **Sem projeto**: Mais simples, funciona igual

Escolha o que preferir! ??
