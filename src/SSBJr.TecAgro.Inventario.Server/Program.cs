using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using SSBJr.TecAgro.Inventario.Domain.Repositories;
using SSBJr.TecAgro.Inventario.Domain.Services;
using SSBJr.TecAgro.Inventario.Infrastructure.Data;
using SSBJr.TecAgro.Inventario.Infrastructure.Repositories;
using SSBJr.TecAgro.Inventario.Infrastructure.Services;
using MediatR;
using System.Reflection;

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/inventario-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Iniciando SSBJr.TecAgro.Inventario.Server");

    var builder = WebApplication.CreateBuilder(args);

    // Adicionar Serilog
    builder.Host.UseSerilog();

    // Adicionar services ao container
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
c.SwaggerDoc("v1", new() 
    { 
            Title = "SSBJr TecAgro Inventário API", 
      Version = "v1",
      Description = "API para gerenciamento de inventário fiscal para agropecuária"
    });
    });

    // Configurar CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
     policy.AllowAnyOrigin()
       .AllowAnyMethod()
      .AllowAnyHeader();
    });
    });

    // Configurar Entity Framework com SQL Server
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Server=db;Database=InventarioDb;User Id=sa;Password=Your_password123;TrustServerCertificate=True;";
    
    builder.Services.AddDbContext<InventarioDbContext>(options =>
        options.UseSqlServer(connectionString));

    // Configurar MediatR
    builder.Services.AddMediatR(cfg => 
    {
        cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
        cfg.RegisterServicesFromAssembly(Assembly.Load("SSBJr.TecAgro.Inventario.Domain"));
    });

    // Registrar repositórios
    builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
    builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
    builder.Services.AddScoped<ILogSincronizacaoRepository, LogSincronizacaoRepository>();

    // Registrar serviços
    var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "SSBJr_TecAgro_Inventario_Secret_Key_2025_MinLength32Chars";
    builder.Services.AddScoped<IAutenticacaoService>(sp => 
        new AutenticacaoService(
          sp.GetRequiredService<IUsuarioRepository>(), 
  jwtSecret));

    var storagePath = builder.Configuration["Storage:Path"] ?? "/app/storage";
    builder.Services.AddScoped<IArmazenamentoService>(sp => 
        new ArmazenamentoLocalService(storagePath));

    builder.Services.AddHttpClient<ISincronizacaoService, SincronizacaoService>();
    builder.Services.AddScoped<ISincronizacaoService>(sp =>
        new SincronizacaoService(
            sp.GetRequiredService<IProdutoRepository>(),
     sp.GetRequiredService<ILogSincronizacaoRepository>(),
            sp.GetRequiredService<IArmazenamentoService>(),
            sp.GetRequiredService<IMediator>(),
   sp.GetRequiredService<IHttpClientFactory>().CreateClient(),
        builder.Configuration["ServerUrl"] ?? "http://localhost:5000"));

    var app = builder.Build();

    // Migrar e seed database
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<InventarioDbContext>();
   
        // Criar banco e tabelas se não existirem (temporário para desenvolvimento)
        db.Database.EnsureCreated();
        
  // Seed usuário padrão se não existir
        var userRepo = scope.ServiceProvider.GetRequiredService<IUsuarioRepository>();
        var authService = scope.ServiceProvider.GetRequiredService<IAutenticacaoService>();
        
        var adminUser = await userRepo.GetByLoginAsync("admin");
        if (adminUser == null)
        {
await userRepo.AddAsync(new SSBJr.TecAgro.Inventario.Domain.Entities.Usuario
 {
          Id = Guid.NewGuid(),
          Login = "admin",
                Nome = "Administrador",
  Email = "admin@tecagro.com",
          SenhaHash = authService.GerarHashSenha("admin123"),
          DataCriacao = DateTime.UtcNow,
      Ativo = true
            });
    Log.Information("Usuário admin criado com sucesso");
        }
    }

    // Configure o HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
      app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Inventário API v1"));
 }

    app.UseSerilogRequestLogging();

    app.UseCors("AllowAll");

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    Log.Information("Servidor iniciado com sucesso na porta {Port}", 
        builder.Configuration["ASPNETCORE_URLS"] ?? "http://localhost:5000");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Aplicação terminou inesperadamente");
}
finally
{
    Log.CloseAndFlush();
}
