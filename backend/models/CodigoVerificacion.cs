// Models/CodigoVerificacion.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class CodigoVerificacion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(5)]
        public string Codigo { get; set; }

        [Required]
        public int user_id { get; set; }

        [Required]
        [MaxLength(10)]
        public string Telefono { get; set; }

        [Required]
        public DateTime Expiracion { get; set; }

        public bool Usado { get; set; }

        [ForeignKey("user_id")]
        public Usuario Usuario { get; set; }
    }
}
