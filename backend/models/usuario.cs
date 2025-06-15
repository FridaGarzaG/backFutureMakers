using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Telefono { get; set; }
    }
}
