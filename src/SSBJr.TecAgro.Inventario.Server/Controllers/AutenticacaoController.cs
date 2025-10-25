using Microsoft.AspNetCore.Mvc;
using SSBJr.TecAgro.Inventario.Domain.Services;

namespace SSBJr.TecAgro.Inventario.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutenticacaoController : ControllerBase
{
  private readonly IAutenticacaoService _autenticacaoService;
    private readonly ILogger<AutenticacaoController> _logger;

    public AutenticacaoController(
        IAutenticacaoService autenticacaoService,
        ILogger<AutenticacaoController> logger)
    {
        _autenticacaoService = autenticacaoService;
 _logger = logger;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        try
 {
    if (string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Senha))
           return BadRequest("Login e senha são obrigatórios");

            var token = await _autenticacaoService.AutenticarAsync(request.Login, request.Senha);

     if (token == null)
     {
          _logger.LogWarning("Tentativa de login inválida para usuário: {Login}", request.Login);
              return Unauthorized("Login ou senha inválidos");
            }

        _logger.LogInformation("Login bem-sucedido para usuário: {Login}", request.Login);

            return Ok(new LoginResponse { Token = token });
        }
    catch (Exception ex)
 {
            _logger.LogError(ex, "Erro no processo de autenticação");
return StatusCode(500, "Erro ao processar login");
  }
    }

    [HttpPost("validar")]
  public async Task<ActionResult<bool>> ValidarToken([FromBody] ValidarTokenRequest request)
{
        try
        {
            var valido = await _autenticacaoService.ValidarTokenAsync(request.Token);
    return Ok(new { Valido = valido });
      }
        catch (Exception ex)
        {
   _logger.LogError(ex, "Erro ao validar token");
      return StatusCode(500, "Erro ao validar token");
     }
    }
}

public record LoginRequest(string Login, string Senha);
public record LoginResponse { public string Token { get; set; } = string.Empty; }
public record ValidarTokenRequest(string Token);
