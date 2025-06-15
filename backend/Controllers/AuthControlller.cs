using Microsoft.AspNetCore.Mvc;
using Vonage;
using Vonage.Messaging;
using Vonage.Request;
using backend.Models;
using backend.Data;
using System;
using System.Linq;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/auth/registro
        [HttpPost("registro")]
        public IActionResult RegistrarUsuario([FromBody] Usuario usuario)
        {
            if (_context.Usuarios.Any(u => u.Nombre == usuario.Nombre))
            {
                return Conflict(new { mensaje = "Este nombre ya está registrado." });
            }

            if (_context.Usuarios.Any(u => u.Telefono == usuario.Telefono))
            {
                return Conflict(new { mensaje = "Este número ya está registrado." });
            }

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            var codigo = GenerarCodigo();

            var clave = new Clave
            {
                CodigoClave = codigo,
                UsuarioId = usuario.Id,
                TelefonoUsuario = usuario.Telefono,
                Usado = false,
                Expiracion = DateTime.Now.AddMinutes(5)
            };

            _context.Claves.Add(clave);
            _context.SaveChanges();

            EnviarCodigoSms(usuario.Telefono, codigo);

            return Ok(new { mensaje = "Usuario registrado exitosamente." });
        }

        // POST: api/auth/solicitar-codigo
        [HttpPost("solicitar-codigo")]
        public IActionResult SolicitarCodigo([FromBody] NombreRequest request)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Nombre == request.Nombre);
            if (usuario == null)
            {
                return NotFound(new { mensaje = "Usuario no encontrado." });
            }

            var codigo = GenerarCodigo();

            var clave = new Clave
            {
                CodigoClave = codigo,
                UsuarioId = usuario.Id,
                TelefonoUsuario = usuario.Telefono,
                Usado = false,
                Expiracion = DateTime.Now.AddMinutes(5)
            };

            _context.Claves.Add(clave);
            _context.SaveChanges();

            EnviarCodigoSms(usuario.Telefono, codigo);

            return Ok(new { mensaje = "Código enviado al número registrado.", codigo = codigo });
        }

        // POST: api/auth/verificar-codigo
        [HttpPost("verificar-codigo")]
        public IActionResult VerificarCodigo([FromBody] VerificarCodigoRequest request)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Nombre == request.Nombre);
            if (usuario == null)
            {
                return Unauthorized(new { mensaje = "Usuario no encontrado." });
            }

            var clave = _context.Claves
                .Where(c => c.UsuarioId == usuario.Id && c.CodigoClave == request.Codigo)
                .OrderByDescending(c => c.Expiracion)
                .FirstOrDefault();

            if (clave == null)
            {
                return Unauthorized(new { mensaje = "Código incorrecto." });
            }

            if (clave.Usado)
            {
                return Unauthorized(new { mensaje = "El código ya fue usado." });
            }

            if (clave.Expiracion < DateTime.Now)
            {
                return Unauthorized(new { mensaje = "El código ha expirado." });
            }

            clave.Usado = true;
            _context.SaveChanges();

            return Ok(new
            {
                mensaje = "Inicio de sesión exitoso.",
                usuario = new
                {
                    usuario.Id,
                    usuario.Nombre,
                    usuario.Telefono
                }
            });
        }

        // Método auxiliar para enviar el código SMS con Vonage
        private void EnviarCodigoSms(string telefono, string codigo)
        {
            var credentials = Credentials.FromApiKeyAndSecret(
                "4f5a272e",                  // Tu API key
                "YZImmO3c6xE4dpTl"           // Tu API secret
            );

            var client = new VonageClient(credentials);

            var smsRequest = new SendSmsRequest
            {
                To = "+52" + telefono,
                From = "Vonage APIs",
                Text = $"Tu código de verificación es: {codigo}"
            };

            var response = client.SmsClient.SendAnSmsAsync(smsRequest).Result;

            if (response.Messages[0].Status != "0")
            {
                Console.WriteLine($"Error al enviar SMS: {response.Messages[0].ErrorText}");
            }
        }

        // Método auxiliar para generar código de 5 dígitos
        private string GenerarCodigo()
        {
            var random = new Random();
            return random.Next(10000, 99999).ToString();
        }
    }

    // DTOs para solicitudes
    public class NombreRequest
    {
        public string Nombre { get; set; } = null!;
    }

    public class VerificarCodigoRequest
    {
        public string Nombre { get; set; } = null!;
        public string Codigo { get; set; } = null!;
    }
}
