using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using inmobiliaria.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Tls;

namespace inmobiliaria.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly IConfiguration configuration;
		private readonly IWebHostEnvironment environment;

		public UsuariosController(IConfiguration configuration, IWebHostEnvironment environment)
		{
			this.configuration = configuration;
			this.environment = environment;
		}
        
        // GET: Usuarios
        [Authorize]
        public ActionResult Index()
        {
           if (User.IsInRole("Empleado"))
			{
				TempData["Mensaje"] = "No tienes permiso de ver este contenido";
                return RedirectToAction(nameof(Index), "Home"); 
                
			}
            ViewBag.Id = TempData["Id"];
            if(TempData.ContainsKey("Mensaje"))
            {
                ViewBag.Mensaje = TempData["Mensaje"];

            }
            var UR = new UsuariosRepositorio();
            var U = UR.ObtenerTodos();
            return View(U);
        }
        [Authorize]
        // GET: Usuarios/Details/5
        public ActionResult Details(int id)
        {
            if (User.IsInRole("Empleado"))
			{
				TempData["Mensaje"] = "No tienes permiso de ver este contenido";
                return RedirectToAction(nameof(Index), "Home"); 
                
			}
            var UR = new UsuariosRepositorio();
            var U = UR.ObtenerXId(id);
            return View(U);
        }
        [Authorize]
        // GET: Usuarios/Create
        public ActionResult Create()
        {
            if (User.IsInRole("Empleado"))
			{
				TempData["Mensaje"] = "No tienes permiso de ver este contenido";
                return RedirectToAction(nameof(Index), "Home"); 
                
			}
            ViewBag.Id = TempData["Id"];
            
            if(TempData.ContainsKey("Mensaje"))
            {
                ViewBag.Mensaje = TempData["Mensaje"];

            }
            var U = new Usuarios();
            return View(U);
        }
        [Authorize]
        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Usuarios u)
        {
            if (User.IsInRole("Empleado"))
			{
				TempData["Mensaje"] = "No tienes permiso de realizar esta accion";
                return RedirectToAction(nameof(Index), "Home"); 
                
			}
            //Validaciones de Entrada 
            if (string.IsNullOrEmpty(u.DNI))
            {
                TempData["Mensaje"]="El campo de DNI es obligatorio";
                ModelState.AddModelError("DNI", "El campo DNI es obligatorio.");
                return RedirectToAction(nameof(Create));
            }else if( !Expresiones.ValidarDNI(u.DNI.ToString())){
                TempData["Mensaje"]="El DNI o pasaporte no es valido";
                ModelState.AddModelError("DNI", "El DNI no es valido, se permiten solo 7 o 8 caracteres.");
                return RedirectToAction(nameof(Create));
            }
            if (string.IsNullOrEmpty(u.Nombre))
            {
                TempData["Mensaje"]="El campo de Nombre es obligatorio";
                ModelState.AddModelError("Nombre", "El campo Nombre es obligatorio.");
                return RedirectToAction(nameof(Create));
            }else if( !Expresiones.ValidarNombre(u.Nombre)){
                TempData["Mensaje"]="El Nombre es valido";
                ModelState.AddModelError("Nombre", "El Nombre es valido");
                return RedirectToAction(nameof(Create));
            }
            if (string.IsNullOrEmpty(u.Apellido))
            {
                TempData["Mensaje"]="El campo de Apellido es obligatorio";
                ModelState.AddModelError("Apellido", "El campo Apellido es obligatorio.");
                return RedirectToAction(nameof(Create));
            } else if(!Expresiones.ValidarApellido(u.Apellido)){
                TempData["Mensaje"]="El Apellido no es valido";
                ModelState.AddModelError("Apellido", "El Apellido no es valido.");
                return RedirectToAction(nameof(Create));
            }
            if (u.Telefono < 0)
            {
                TempData["Mensaje"]="El campo de Telefono es obligatorio";
                ModelState.AddModelError("Telefono", "El campo Telefono es obligatorio.");
                return RedirectToAction(nameof(Create));
            }else if(!Expresiones.ValidarTelefono(u.Telefono.ToString())){
                TempData["Mensaje"]="El Telefono no es valido";
                ModelState.AddModelError("Telefono", "El Telefono no es valido.");
                return RedirectToAction(nameof(Create));
            }
            if (string.IsNullOrEmpty(u.Mail))
            {
                TempData["Mensaje"]="El campo de Mail es obligatorio";
                ModelState.AddModelError("Mail", "El campo Mail es obligatorio.");
                return RedirectToAction(nameof(Create));
            }else if(!Expresiones.ValidarMail(u.Mail)){
                 TempData["Mensaje"]="El Mail es invalido, deberia estar en formato 'example@example.com'";
                ModelState.AddModelError("Mail", "El Mail es invalido");
                return RedirectToAction(nameof(Create));
            }
            try
            {
                var UR = new UsuariosRepositorio();
                //Primera Mayuscula Nombre y Apelldo
                u.Nombre = char.ToUpper(u.Nombre[0]) + u.Nombre.Substring(1).ToLower();
                u.Apellido = char.ToUpper(u.Apellido[0]) + u.Apellido.Substring(1).ToLower();
                UR.Alta(u);
                TempData["Id"] = u.Id;
                return RedirectToAction(nameof(Index));
            }
            catch(Exception e)
            {
                TempData["Mensaje"] = e.Message;
                Console.WriteLine(e.Message);
                return RedirectToAction(nameof(Create));
            }
        }
        [Authorize]
        // GET: Usuarios/Edit/5
        public ActionResult Edit(int id)
        {
            if (User.IsInRole("Empleado"))
			{
				if (User.Identity.Name != id+"")
                {
                    TempData["Mensaje"] = "No tienes permiso de realizar esta accion";
                    return RedirectToAction(nameof(Index), "Home"); 
                }
					
			}
            var UR = new UsuariosRepositorio();
            
            ViewBag.Id = TempData["Id"];
            if(TempData.ContainsKey("Mensaje"))
            {
                ViewBag.Mensaje = TempData["Mensaje"];

            }
            return View(UR.ObtenerXId(id));
        }
        [Authorize]
        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Usuarios u)
        {
            if (User.IsInRole("Empleado"))
			{
				if (User.Identity.Name != id+"")
                {
                    TempData["Mensaje"] = "No tienes permiso de realizar esta accion";
                    return RedirectToAction(nameof(Index), "Home"); 
                }
					
			}
             //Validaciones de Entrada 
            if (string.IsNullOrEmpty(u.DNI))
            {
                TempData["Mensaje"]="El campo de DNI es obligatorio";
                ModelState.AddModelError("DNI", "El campo DNI es obligatorio.");
                return RedirectToAction(nameof(Create));
            }else if( !Expresiones.ValidarDNI(u.DNI.ToString())){
                TempData["Mensaje"]="El DNI no es valido, se permiten solo 7 o 8 caracteres";
                ModelState.AddModelError("DNI", "El DNI no es valido, se permiten solo 7 o 8 caracteres.");
                return RedirectToAction(nameof(Create));
            }
            if (string.IsNullOrEmpty(u.Nombre))
            {
                TempData["Mensaje"]="El campo de Nombre es obligatorio";
                ModelState.AddModelError("Nombre", "El campo Nombre es obligatorio.");
                return RedirectToAction(nameof(Create));
            }else if( !Expresiones.ValidarNombre(u.Nombre)){
                TempData["Mensaje"]="El Nombre es valido";
                ModelState.AddModelError("Nombre", "El Nombre es valido");
                return RedirectToAction(nameof(Create));
            }
            if (string.IsNullOrEmpty(u.Apellido))
            {
                TempData["Mensaje"]="El campo de Apellido es obligatorio";
                ModelState.AddModelError("Apellido", "El campo Apellido es obligatorio.");
                return RedirectToAction(nameof(Create));
            } else if(!Expresiones.ValidarApellido(u.Apellido)){
                TempData["Mensaje"]="El Apellido no es valido";
                ModelState.AddModelError("Apellido", "El Apellido no es valido.");
                return RedirectToAction(nameof(Create));
            }
            if (u.Telefono < 0)
            {
                TempData["Mensaje"]="El campo de Telefono es obligatorio";
                ModelState.AddModelError("Telefono", "El campo Telefono es obligatorio.");
                return RedirectToAction(nameof(Create));
            }else if(!Expresiones.ValidarTelefono(u.Telefono.ToString())){
                TempData["Mensaje"]="El Telefono no es valido";
                ModelState.AddModelError("Telefono", "El Telefono no es valido.");
                return RedirectToAction(nameof(Create));
            }
            try
            {   
                var UR = new UsuariosRepositorio();
                var ER = new EmpleadosRepositorio();
                //Primera Mayuscula Nombre y Apelldo
                u.Nombre = char.ToUpper(u.Nombre[0]) + u.Nombre.Substring(1).ToLower();
                u.Apellido = char.ToUpper(u.Apellido[0]) + u.Apellido.Substring(1).ToLower();
                var bol =UR.Modificacion(id,u);
                if(bol)
                {
                  TempData["Mensaje"]="Se modifico con exito la entidad id:"+u.Id;
                return RedirectToAction(nameof(Index),"Home");  
                }else
                {
                    throw new Exception("No se pudo modificar con exito la entidad con id:"+u.Id);
                }
                
            }
            catch(Exception e)
            {
                TempData["Mensaje"] = e.Message;
                Console.WriteLine(e.Message);
                return RedirectToAction(nameof(Edit));
            }
        }
        [Authorize]
        // GET: Usuarios/Delete/5
        public ActionResult Delete(int id)
        {
            if (User.IsInRole("Empleado"))
                {
                        TempData["Mensaje"] = "No tienes permiso de realizar esta accion";
                        return RedirectToAction(nameof(Index), "Home"); 
            }
            ViewBag.Id = TempData["Id"];
            
            if(TempData.ContainsKey("Mensaje"))
            {
                ViewBag.Mensaje = TempData["Mensaje"];

            }
            
            var UR = new UsuariosRepositorio();
                
            return View(UR.ObtenerXId(id));
        }
        
        // POST: Usuarios/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Delete(int id, Usuarios u)
        {
            if (User.IsInRole("Empleado"))
            {
                TempData["Mensaje"] = "No tienes permiso de realizar esta accion";
                return RedirectToAction(nameof(Index), "Home"); 
            }
            try
            {
                var UR = new UsuariosRepositorio();
                
                var bol = UR.Baja(UR.ObtenerXId(id));
                if(bol)
                {
                    TempData["Mensaje"] = "Se elimino con exito la entidad";
                    return RedirectToAction(nameof(Index));

                }else{
                    throw new Exception("No se logro eliminar la entidad id: "+id);
                }
            }
            catch(Exception e)
            {
                TempData["Mensaje"] = e.Message;
                Console.WriteLine(e.Message);
                return RedirectToAction(nameof(Delete));
            }
        }
        public ActionResult Login()
        {
           
            if(TempData.ContainsKey("Mensaje"))
            {
                ViewBag.Mensaje = TempData["Mensaje"];

            }
            
            var UE = new UsuariosEspeciales();
            return View(UE);
        }
        // POST: Usuarios/Login/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(UsuariosEspeciales ue){
            //Validaciones de Entrada
            if (string.IsNullOrEmpty(ue.UsuarioId.Mail))
                {
                    TempData["Mensaje"]="El campo de Mail es obligatorio";
                    ModelState.AddModelError("UsuarioId.Mail", "El campo Mail es obligatorio.");
                    return RedirectToAction(nameof(Login));
                }else if (!Expresiones.ValidarMail(ue.UsuarioId.Mail))
                {
                    TempData["Mensaje"]="Debes ingresar un mail valido";
                    ModelState.AddModelError("UsuarioId.Mail", "Mail invalido");
                    return RedirectToAction(nameof(Login));
                }
                if (string.IsNullOrEmpty(ue.Clave))
                {
                    TempData["Mensaje"]="El campo de Clave es obligatorio";
                    ModelState.AddModelError("Clave", "El campo Clave es obligatorio.");
                return RedirectToAction(nameof(Login));
                }    
            try
            {
                var UR = new UsuariosRepositorio();
                var u = UR.ObtenerXMail(ue.UsuarioId.Mail);
                    
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: ue.Clave,
                    salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 250 / 8
                ));
                if(u == null)
                {
                    TempData["Mensaje"] = "Mail o Clave incorrecta";  
                    return RedirectToAction(nameof(Login));
                }
                if(u.Clave != hashed)
                {
                    TempData["Mensaje"] = "Mail o Clave incorrecta";  
                    return RedirectToAction(nameof(Login));
                }
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, u.Id+""),
                    new Claim("FullName",u.UsuarioId.toString()),
                    new Claim(ClaimTypes.Role , u.rol),
                };
                    
                var claimIdentity = new ClaimsIdentity(
                    claims,CookieAuthenticationDefaults.AuthenticationScheme
                );
                    
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimIdentity)
                );
                TempData["Mensaje"] = "Bienvenido "+u.UsuarioId.Nombre+" "+u.UsuarioId.Apellido;
                return RedirectToAction("Index","Home");
            }
            catch(Exception e)
            {
                TempData["Mensaje"] = e.Message;
                Console.WriteLine(e.Message);
                return RedirectToAction(nameof(Login));
            }
        }
        
        [Authorize]
        public async Task<ActionResult> Logout(){
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index","Home");
        }  
    }
}