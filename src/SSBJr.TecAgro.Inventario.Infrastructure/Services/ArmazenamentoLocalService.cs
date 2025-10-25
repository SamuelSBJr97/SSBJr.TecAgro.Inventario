using SSBJr.TecAgro.Inventario.Domain.Services;

namespace SSBJr.TecAgro.Inventario.Infrastructure.Services;

public class ArmazenamentoLocalService : IArmazenamentoService
{
    private readonly string _basePath;

    public ArmazenamentoLocalService(string basePath)
    {
        _basePath = basePath;
     Directory.CreateDirectory(_basePath);
 }

    public async Task<string> SalvarFotoAsync(byte[] fotoBytes, string nomeArquivo)
    {
  var caminhoCompleto = Path.Combine(_basePath, nomeArquivo);
 await File.WriteAllBytesAsync(caminhoCompleto, fotoBytes);
        return caminhoCompleto;
  }

public async Task<byte[]> ObterFotoAsync(string caminhoFoto)
    {
    return await File.ReadAllBytesAsync(caminhoFoto);
 }

    public Task DeletarFotoAsync(string caminhoFoto)
    {
     if (File.Exists(caminhoFoto))
            File.Delete(caminhoFoto);
        
     return Task.CompletedTask;
    }
}
