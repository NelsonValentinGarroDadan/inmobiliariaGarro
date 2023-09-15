using System.ComponentModel.DataAnnotations;

namespace inmobiliaria.Models;

public class Tipos{
    [Key]
    [Display(Name = "Codigo int.")]
    [Required]
    public int Id { get; set;}
    [Required]
    public string? Descripcion { get; set;}
  

}