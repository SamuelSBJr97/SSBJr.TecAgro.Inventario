using Microsoft.EntityFrameworkCore;
using SSBJr.TecAgro.Inventario.Domain.Entities;

namespace SSBJr.TecAgro.Inventario.Infrastructure.Data;

public class InventarioDbContext : DbContext
{
    public InventarioDbContext(DbContextOptions<InventarioDbContext> options) : base(options)
    {
    }

public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<LogSincronizacao> LogsSincronizacao => Set<LogSincronizacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
   base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Produto>(entity =>
        {
            entity.HasKey(e => e.Id);
  entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CodigoFiscal).HasMaxLength(50);
 entity.Property(e => e.SKU).HasMaxLength(50);
      entity.Property(e => e.Categoria).HasMaxLength(100);
  entity.Property(e => e.UnidadeMedida).HasMaxLength(20);
 entity.Property(e => e.Localizacao).HasMaxLength(200);
            entity.Property(e => e.ValorAquisicao).HasPrecision(18, 2);
            entity.Property(e => e.ValorRevenda).HasPrecision(18, 2);
            entity.Property(e => e.QuantidadeEstoque).HasPrecision(18, 3);
            entity.Property(e => e.Fotos).HasConversion(
       v => string.Join(';', v),
 v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList());
            
   entity.HasIndex(e => e.CodigoFiscal);
     entity.HasIndex(e => e.SKU);
      entity.HasIndex(e => e.StatusSincronizacao);
        });

      modelBuilder.Entity<Usuario>(entity =>
  {
     entity.HasKey(e => e.Id);
  entity.Property(e => e.Login).IsRequired().HasMaxLength(100);
      entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(200);
          entity.Property(e => e.SenhaHash).IsRequired();
 
   entity.HasIndex(e => e.Login).IsUnique();
 });

     modelBuilder.Entity<LogSincronizacao>(entity =>
        {
            entity.HasKey(e => e.Id);
         entity.Property(e => e.Mensagem).HasMaxLength(500);
      
    entity.HasIndex(e => e.ProdutoId);
entity.HasIndex(e => e.DataHora);
        });
    }
}
