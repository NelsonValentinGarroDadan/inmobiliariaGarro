using System.ComponentModel.DataAnnotations;

namespace inmobiliaria.Models;

public class Inmuebles
{
    [Key]
    [Display(Name = "Codigo int.")]
    public int Id { get; set;}
    [Required]
    public string Direccion { get; set;}
    [Required]
    public TiposUsos TipoUsoId{ get; set;}
    [Required]
    public int CA { get; set;}
    [Required]
    public string Latitud { get; set;}
    [Required]
    public string Longitud { get; set;}
    [Required]
    public decimal Precio { get; set;}
    [Required]
    public DateTime FechaInicio { get; set;}
    [Required]
    public DateTime FechaFin{ get; set;}
    [Required]
    public TiposEstados TipoEstadoId { get; set;}
    [Required]
    public Propietarios PropietarioId { get; set;}
    [Required]
    public TiposInmuebles TipoInmuebleId {get; set;}
    public string toString(){
        return "Id: "+Id+" | Direccion: "+Direccion+" | Dueño: ( "+PropietarioId.toString()+" )";
    }
}
