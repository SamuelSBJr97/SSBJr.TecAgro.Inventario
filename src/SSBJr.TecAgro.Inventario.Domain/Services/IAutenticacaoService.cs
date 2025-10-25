namespace SSBJr.TecAgro.Inventario.Domain.Services;

public interface IAutenticacaoService
{
    Task<string?> AutenticarAsync(string login, string senha);
    Task<bool> ValidarTokenAsync(string token);
    string GerarHashSenha(string senha);
    bool VerificarSenha(string senha, string hash);
}
