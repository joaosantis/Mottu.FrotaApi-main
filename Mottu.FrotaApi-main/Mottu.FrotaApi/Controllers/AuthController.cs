using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Mottu.FrotaApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Realiza login e retorna um token JWT válido por 60 minutos.
        /// </summary>
        /// <param name="login">Usuário e senha para autenticação.</param>
        /// <returns>Token JWT válido.</returns>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            // 🔐 Simulação simples (usuário fixo)
            if (login.Username == "admin" && login.Password == "123")
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, login.Username),
                        new Claim("role", "Administrador")
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:ExpireMinutes"] ?? "60")),
                    Issuer = _config["Jwt:Issuer"],
                    Audience = _config["Jwt:Audience"],
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var jwt = tokenHandler.WriteToken(token);

                return Ok(new
                {
                    message = "Login realizado com sucesso!",
                    token = jwt,
                    expiresAt = tokenDescriptor.Expires
                });
            }

            return Unauthorized(new { message = "Usuário ou senha inválidos." });
        }
    }

    /// <summary>
    /// Modelo de login simples para autenticação.
    /// </summary>
    public class LoginModel
    {
        
        public string Username { get; set; } = string.Empty;

       
        public string Password { get; set; } = string.Empty;
    }
}
