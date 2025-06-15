using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("productos")]  // Opcional si ya lo pusiste en el DbContext
    public class Producto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("nombre")]
        public string Nombre { get; set; }

        [MaxLength(255)]
        [Column("imagen")]
        public string Imagen { get; set; }

        // Dos tamaños distintos según la tabla:
        [Column("tamano_ml")]
        public int TamanoMl { get; set; }

        [Column("tamano_l", TypeName = "decimal(5,2)")]
        public decimal TamanoL { get; set; }

        [Column("precio", TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        [Column("cantidad_disponible")]
        public int CantidadDisponible { get; set; }
    }
}
