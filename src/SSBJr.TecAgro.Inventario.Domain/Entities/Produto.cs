namespace SSBJr.TecAgro.Inventario.Domain.Entities;

public class Produto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string CodigoFiscal { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public decimal QuantidadeEstoque { get; set; }
    public string UnidadeMedida { get; set; } = string.Empty;
    public decimal ValorAquisicao { get; set; }
    public decimal ValorRevenda { get; set; }
    public string Localizacao { get; set; } = string.Empty;
    public List<string> Fotos { get; set; } = new();
    public DateTime DataCadastro { get; set; }
    public DateTime DataAtualizacao { get; set; }
    public StatusSincronizacao StatusSincronizacao { get; set; }
 public string? ErroSincronizacao { get; set; }
    public bool Ativo { get; set; } = true;
}

public enum StatusSincronizacao
{
    Pendente = 0,
    Sincronizado = 1,
    Erro = 2,
    EmProcessamento = 3
}
