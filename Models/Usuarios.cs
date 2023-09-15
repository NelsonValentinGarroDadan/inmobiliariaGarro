using System.ComponentModel.DataAnnotations;

namespace inmobiliaria.Models;

public class Usuarios
{
    [Key]
    [Display(Name = "Codigo int.")]
    public int Id { get; set;}
    [Required]
    public int DNI { get; set;}
    [Required]
    public string? Nombre { get; set;}
    [Required]
    public string? Apellido { get; set;}
    [Required]
    public long Telefono { get; set;}
    [Required]
    public string? Mail { get; set;}
    public string toString() => "Id: "+Id+" | FullName: "+Nombre+" "+Apellido+" | DNI: "+DNI;
}
