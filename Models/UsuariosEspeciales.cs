using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inmobiliaria.Models;

public class UsuariosEspeciales
{ 
    [Key]
    [Display(Name = "Codigo int.")]
    public int Id { get; set;}
    public Usuarios UsuarioId{ get; set;}
    [Required]
    public string Clave { get; set;}
    [NotMapped]
    public string? rol {get;set;}
    [NotMapped]
    public IFormFile AvatarFileName { get; set; }
    public string? Avatar { get; set; }
}