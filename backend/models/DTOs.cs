namespace backend.Models
{
    public class NombreRequest
    {
        public string Nombre { get; set; } = null!;
    }

    public class VerificarClaveRequest
    {
        public string Nombre { get; set; } = null!;
        public string Clave { get; set; } = null!;
    }
}
