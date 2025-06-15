using Microsoft.AspNetCore.Mvc;
using Vonage;
using Vonage.Messaging;
using Vonage.Request;
using backend.Models;
using backend.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        [HttpPost("enviar-codigo")]
        public async Task<IActionResult> EnviarCodigo([FromBody] TelefonoRequest request)
        {
            var codigo = GenerarCodigo();

            var credentials = Credentials.FromApiKeyAndSecret(
                "4f5a272e",                 // ← Tu API Key
                "YZImmO3c6xE4dpTl"          // ← Tu API Secret
            );

            var client = new VonageClient(credentials);

            var smsRequest = new SendSmsRequest
            {
                To = "+52" + request.Telefono,
                From = "Vonage APIs",
                Text = $"Tu código de verificación es: {codigo}"
            };

            var response = await client.SmsClient.SendAnSmsAsync(smsRequest);

            if (response.Messages[0].Status != "0")
            {
                return BadRequest(new { error = response.Messages[0].ErrorText });
            }

            return Ok(new { mensaje = "Código enviado correctamente", codigo });
        }

        [HttpPost("registro")]
        public IActionResult RegistrarUsuario([FromBody] Usuario usuario)
        {
            // Validar nombre único
            if (_context.Usuarios.Any(u => u.Nombre == usuario.Nombre))
            {
                return Conflict(new { mensaje = "Este nombre ya está registrado." });
            }

            // Validar teléfono único
            if (_context.Usuarios.Any(u => u.Telefono == usuario.Telefono))
            {
                return Conflict(new { mensaje = "Este número ya está registrado." });
            }

            // Guardar usuario
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            // Generar código y guardar en tabla de verificación
            var codigo = GenerarCodigo();
            var codigoVerificacion = new CodigoVerificacion
            {
                user_id = usuario.Id,
                Codigo = codigo,
                Telefono = usuario.Telefono,
                Expiracion = DateTime.Now.AddMinutes(5),
                Usado = false
            };

            _context.CodigosVerificacion.Add(codigoVerificacion);
            _context.SaveChanges();

            // Enviar código vía SMS
            EnviarCodigoSms(usuario.Telefono, codigo);

            return Ok(new { mensaje = "Usuario registrado exitosamente", codigoEnviado = codigo });
        }

        // Genera un código aleatorio de 5 caracteres
        private string GenerarCodigo()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 5)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // Envía un SMS utilizando Vonage
        private void EnviarCodigoSms(string telefono, string codigo)
        {
            var credentials = Credentials.FromApiKeyAndSecret(
                "4f5a272e",
                "YZImmO3c6xE4dpTl"
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
    }
}