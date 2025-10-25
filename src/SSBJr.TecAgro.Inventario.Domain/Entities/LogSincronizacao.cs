namespace SSBJr.TecAgro.Inventario.Domain.Entities;

public class LogSincronizacao
{
    public Guid Id { get; set; }
    public Guid ProdutoId { get; set; }
    public DateTime DataHora { get; set; }
    public StatusSincronizacao Status { get; set; }
    public string? Mensagem { get; set; }
    public string? Detalhes { get; set; }
}
