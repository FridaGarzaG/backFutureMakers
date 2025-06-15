using System.ComponentModel.DataAnnotations.Schema;

[Table("codigos_verificacion")]
public class CodigoVerificacion
{
    public int Id { get; set; }
    public string Codigo { get; set; }
    public DateTime Expiracion { get; set; }
    public string Telefono { get; set; }
    public bool Usado { get; set; }
    public int user_id { get; set; }
}
