using MediatR;

namespace SSBJr.TecAgro.Inventario.Domain.Events;

public record ProdutoCriadoEvent(Guid ProdutoId, string Nome, DateTime DataCriacao) : INotification;

public record ProdutoAtualizadoEvent(Guid ProdutoId, DateTime DataAtualizacao) : INotification;

public record ProdutoSincronizadoEvent(Guid ProdutoId, DateTime DataSincronizacao) : INotification;

public record ErroSincronizacaoEvent(Guid ProdutoId, string Mensagem, DateTime DataErro) : INotification;
