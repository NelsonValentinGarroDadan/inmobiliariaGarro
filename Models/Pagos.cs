using System.ComponentModel.DataAnnotations;
namespace inmobiliaria.Models;

public class Pagos 
{
    [Key]
    [Display(Name = "Codigo int.")]
    public int Id {set; get;}
    [Required]
    public DateTime Fecha {set;get;}
    [Required]
    public Contratos ContratoId { set; get; }
    [Required]
    public decimal Importe {set;get;}
    

}