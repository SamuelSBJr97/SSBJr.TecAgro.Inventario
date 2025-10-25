using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using SSBJr.TecAgro.Inventario.Domain.Repositories;
using SSBJr.TecAgro.Inventario.Domain.Services;

namespace SSBJr.TecAgro.Inventario.Infrastructure.Services;

public class AutenticacaoService : IAutenticacaoService
{
  private readonly IUsuarioRepository _usuarioRepository;
    private readonly string _jwtSecret;

    public AutenticacaoService(IUsuarioRepository usuarioRepository, string jwtSecret = "SSBJr_TecAgro_Inventario_Secret_Key_2025_MinLength32Chars")
    {
        _usuarioRepository = usuarioRepository;
        _jwtSecret = jwtSecret;
    }

    public async Task<string?> AutenticarAsync(string login, string senha)
    {
        var usuario = await _usuarioRepository.GetByLoginAsync(login);
        
     if (usuario == null || !VerificarSenha(senha, usuario.SenhaHash))
            return null;

        // Atualizar último acesso
        usuario.UltimoAcesso = DateTime.UtcNow;
        await _usuarioRepository.UpdateAsync(usuario);

        // Gerar JWT token
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSecret);
     var tokenDescriptor = new SecurityTokenDescriptor
        {
   Subject = new ClaimsIdentity(new[]
            {
         new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
    new Claim(ClaimTypes.Name, usuario.Login),
     new Claim(ClaimTypes.Email, usuario.Email)
        }),
Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(
    new SymmetricSecurityKey(key),
         SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
    }

    public async Task<bool> ValidarTokenAsync(string token)
    {
      try
   {
       var tokenHandler = new JwtSecurityTokenHandler();
       var key = Encoding.ASCII.GetBytes(_jwtSecret);
         
       tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
 ValidateIssuerSigningKey = true,
         IssuerSigningKey = new SymmetricSecurityKey(key),
              ValidateIssuer = false,
    ValidateAudience = false,
             ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

   return validatedToken != null;
        }
catch
        {
    return false;
        }
    }

    public string GerarHashSenha(string senha)
    {
      using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(senha);
        var hash = sha256.ComputeHash(bytes);
  return Convert.ToBase64String(hash);
    }

    public bool VerificarSenha(string senha, string hash)
    {
     var senhaHash = GerarHashSenha(senha);
  return senhaHash == hash;
    }
}
