using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class Clave
    {
        [Key]
        public int Id { get; set; }

        [StringLength(5)]
        [Column(TypeName = "char(5)")]
        public required string CodigoClave { get; set; } 

        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }

        [StringLength(10)]
        [Column(TypeName = "char(10)")]
        public required string TelefonoUsuario { get; set; }  // marcar como required para evitar warnings

        public bool Usado { get; set; } = false;

        public DateTime Expiracion { get; set; }

        public Usuario? Usuario { get; set; }  // required para evitar warnings de nullable
    }
}
