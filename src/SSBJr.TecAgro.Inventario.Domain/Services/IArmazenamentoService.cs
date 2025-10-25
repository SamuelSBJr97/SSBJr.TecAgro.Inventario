namespace SSBJr.TecAgro.Inventario.Domain.Services;

public interface IArmazenamentoService
{
    Task<string> SalvarFotoAsync(byte[] fotoBytes, string nomeArquivo);
    Task<byte[]> ObterFotoAsync(string caminhoFoto);
    Task DeletarFotoAsync(string caminhoFoto);
}
