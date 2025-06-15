using Microsoft.AspNetCore.Mvc;
using Vonage;
using Vonage.Messaging;
using Vonage.Request;
using backend.Models;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        [HttpPost("enviar-codigo")]
        public async Task<IActionResult> EnviarCodigo([FromBody] TelefonoRequest request)
        {
            var codigo = GenerarCodigo();

            var credentials = Credentials.FromApiKeyAndSecret(
                "4f5a272e",
                "YZImmO3c6xE4dpTl"
            );

            var client = new VonageClient(credentials);

            var smsRequest = new SendSmsRequest
            {
                To = "+52" + request.Telefono,
                From = "Vonage APIs",
                Text = $"Tu codigo de verificacion es: {codigo}"
            };

            var response = await client.SmsClient.SendAnSmsAsync(smsRequest);

            if (response.Messages[0].Status != "0")
            {
                return BadRequest(new { error = response.Messages[0].ErrorText });
            }

            return Ok(new { mensaje = "Codigo enviado correctamente", codigo });
        }

        private string GenerarCodigo()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 5)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}