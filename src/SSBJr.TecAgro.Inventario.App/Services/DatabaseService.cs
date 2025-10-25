using SQLite;
using SSBJr.TecAgro.Inventario.Domain.Entities;

namespace SSBJr.TecAgro.Inventario.App.Services;

public class DatabaseService : IDatabaseService
{
    private SQLiteAsyncConnection? _database;

    private async Task Init()
    {
        if (_database is not null)
       return;

    var databasePath = Path.Combine(FileSystem.AppDataDirectory, "inventario.db3");
  _database = new SQLiteAsyncConnection(databasePath);
        
      await _database.CreateTableAsync<Produto>();
   await _database.CreateTableAsync<LogSincronizacao>();
    }

 #region Produtos

    public async Task<List<Produto>> GetProdutosAsync()
    {
    await Init();
   return await _database!.Table<Produto>()
     .Where(p => p.Ativo)
     .OrderByDescending(p => p.DataAtualizacao)
     .ToListAsync();
    }

    public async Task<Produto?> GetProdutoAsync(Guid id)
    {
     await Init();
  return await _database!.Table<Produto>()
  .Where(p => p.Id == id)
 .FirstOrDefaultAsync();
    }

    public async Task<int> SaveProdutoAsync(Produto produto)
    {
        await Init();
  
        if (produto.DataCadastro == DateTime.MinValue)
        {
     produto.DataCadastro = DateTime.UtcNow;
 }
        
        produto.DataAtualizacao = DateTime.UtcNow;

 var existing = await GetProdutoAsync(produto.Id);
        
      if (existing != null)
        {
  return await _database!.UpdateAsync(produto);
        }
        else
   {
  return await _database!.InsertAsync(produto);
  }
    }

    public async Task<int> DeleteProdutoAsync(Produto produto)
    {
        await Init();
        // Soft delete
        produto.Ativo = false;
      return await _database!.UpdateAsync(produto);
    }

    public async Task<List<Produto>> SearchProdutosAsync(string searchText)
    {
        await Init();
        
        searchText = searchText.ToLower();
        
 return await _database!.Table<Produto>()
     .Where(p => p.Ativo && 
 (p.Nome.ToLower().Contains(searchText) ||
            p.Descricao.ToLower().Contains(searchText) ||
     p.CodigoFiscal.ToLower().Contains(searchText) ||
     p.SKU.ToLower().Contains(searchText) ||
        p.Categoria.ToLower().Contains(searchText)))
      .OrderBy(p => p.Nome)
      .ToListAsync();
    }

    public async Task<List<Produto>> GetProdutosPendentesAsync()
    {
        await Init();
      return await _database!.Table<Produto>()
  .Where(p => p.StatusSincronizacao == StatusSincronizacao.Pendente)
      .ToListAsync();
    }

    #endregion

    #region Logs

    public async Task<int> SaveLogAsync(LogSincronizacao log)
    {
        await Init();
      return await _database!.InsertAsync(log);
    }

    public async Task<List<LogSincronizacao>> GetLogsAsync()
    {
      await Init();
  return await _database!.Table<LogSincronizacao>()
       .OrderByDescending(l => l.DataHora)
    .Take(100)
  .ToListAsync();
    }

    public async Task<List<LogSincronizacao>> GetLogsByProdutoAsync(Guid produtoId)
    {
        await Init();
  return await _database!.Table<LogSincronizacao>()
   .Where(l => l.ProdutoId == produtoId)
    .OrderByDescending(l => l.DataHora)
    .ToListAsync();
    }

    #endregion
}
