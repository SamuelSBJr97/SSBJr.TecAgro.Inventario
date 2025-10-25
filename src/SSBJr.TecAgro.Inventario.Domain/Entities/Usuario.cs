namespace SSBJr.TecAgro.Inventario.Domain.Entities;

public class Usuario
{
    public Guid Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
 public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }
    public DateTime? UltimoAcesso { get; set; }
    public bool Ativo { get; set; } = true;
}
