using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inmobiliaria.Models;

public class Inquilinos
{ 
    [Key]
    [Display(Name = "Codigo int.")]
    public int Id { get; set;}
    [Required]
    public Usuarios UsuarioId{ get; set;}
    public string toString() => UsuarioId.toString();
}