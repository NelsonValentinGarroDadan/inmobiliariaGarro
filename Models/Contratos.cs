using System.ComponentModel.DataAnnotations;
namespace inmobiliaria.Models;
public class Contratos
{
    [Key]
    [Display(Name = "Codigo int.")]
    public int Id { get; set;}
    [Required]
    public DateTime FechaInicio { get; set;}
    [Required]
    public DateTime FechaFin{ get; set;}
    [Required]
    public Inquilinos InquilinoId { get; set;}
    [Required]
    public Inmuebles InmuebleId{ get; set;}
    public TiposEstados TipoEstadoId { get; set;}

    public decimal Importe { get; set;}
    
    public string toString(){
        return "Id: "+Id+" | Inmueble: ( "+ InmuebleId.toString()+") Inquilino: ( "+InquilinoId.toString()+" )";
    }
}
