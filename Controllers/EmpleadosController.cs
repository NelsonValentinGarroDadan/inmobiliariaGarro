using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliaria.Controllers
{
    public class EmpleadosController : Controller
    {
         private readonly IConfiguration configuration;
		private readonly IWebHostEnvironment environment;

		public EmpleadosController(IConfiguration configuration, IWebHostEnvironment environment)
		{
			this.configuration = configuration;
			this.environment = environment;
		}
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
            var PR = new EmpleadosRepositorio();
            
            return View(PR.ObtenerTodos());
        }

        // GET Empleados/Details/5
        [Authorize]
        public ActionResult Details(int id)
        {
           if (User.IsInRole("Empleado"))
			{
				TempData["Mensaje"] = "No tienes permiso de ver este contenido";
                return RedirectToAction(nameof(Index), "Home"); 
                
			}
            var UR = new UsuariosRepositorio();
            var PR = new EmpleadosRepositorio();
            var E = PR.ObtenerXId(id);
            return View("../Usuarios/Details",E.UsuarioId);
        }

        [Authorize]
        public ActionResult Perfil(int id)
        {
           
            var ER = new EmpleadosRepositorio();
            if (User.IsInRole("Empleado"))
			{
				if (User.Identity.Name != id+"")
                {
                    TempData["Mensaje"] = "No tienes permiso de ver este contenido";
                    return RedirectToAction(nameof(Index), "Home"); 
                }
					
			}
            var PR = new EmpleadosRepositorio();
            var E = PR.ObtenerXId(id);
            return View(E);
        }

        // GET Empleados/Create
        [Authorize]
        public ActionResult Create()
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
            var E = new Empleados();
            ViewBag.Usuarios = UR.ObtenerTodos();
            return View(E);
        }

        // POST Empleados/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Empleados e)
        {
            if (User.IsInRole("Empleado"))
                {
                        TempData["Mensaje"] = "No tienes permiso de realizar esta accion";
                        return RedirectToAction(nameof(Index), "Home"); 
            }
            //Validaciones de Entrada 
            if(e.Id > 0 && (e.UsuarioId.DNI > 0 || !String.IsNullOrEmpty(e.UsuarioId.Nombre) || !String.IsNullOrEmpty(e.UsuarioId.Apellido) || e.UsuarioId.Telefono != 0 || !String.IsNullOrEmpty(e.UsuarioId.Mail)))
            {
               TempData["Mensaje"]="Debes Elegir un Usuario o crear uno, no puede hacer ambos";
                return RedirectToAction(nameof(Create)); 
            }
            if(e.Id < 0 && e.UsuarioId.DNI < 0 && String.IsNullOrEmpty(e.UsuarioId.Nombre) && !String.IsNullOrEmpty(e.UsuarioId.Apellido) && e.UsuarioId.Telefono != 0 && !String.IsNullOrEmpty(e.UsuarioId.Mail))
            {
                TempData["Mensaje"]="Debes Elegir un Usuario o crear uno";
                return RedirectToAction(nameof(Create)); 
            }
            if (e.Id < 0 && e.UsuarioId.DNI < 0)
            {
                TempData["Mensaje"]="El campo de DNI es obligatorio";
                ModelState.AddModelError("UsuarioId.DNI", "El campo DNI es obligatorio.");
                return RedirectToAction(nameof(Create));
            }else if(e.Id < 0 && !Expresiones.ValidarDNI(e.UsuarioId.DNI.ToString())){
                TempData["Mensaje"]="El DNI no es valido, se permiten solo 7 o 8 caracteres";
                ModelState.AddModelError("UsuarioId.DNI", "El DNI no es valido, se permiten solo 7 o 8 caracteres.");
                return RedirectToAction(nameof(Create));
            }
            if (e.Id < 0 && string.IsNullOrEmpty(e.UsuarioId.Nombre))
            {
                TempData["Mensaje"]="El campo de Nombre es obligatorio";
                ModelState.AddModelError("UsuarioId.Nombre", "El campo Nombre es obligatorio.");
                return RedirectToAction(nameof(Create));
            }else if(e.Id < 0 && !Expresiones.ValidarNombre(e.UsuarioId.Nombre)){
                TempData["Mensaje"]="El Nombre es valido";
                ModelState.AddModelError("UsuarioId.Nombre", "El Nombre es valido");
                return RedirectToAction(nameof(Create));
            }
            if (e.Id < 0 && string.IsNullOrEmpty(e.UsuarioId.Apellido))
            {
                TempData["Mensaje"]="El campo de Apellido es obligatorio";
                ModelState.AddModelError("UsuarioId.Apellido", "El campo Apellido es obligatorio.");
                return RedirectToAction(nameof(Create));
            } else if(e.Id < 0 && !Expresiones.ValidarApellido(e.UsuarioId.Apellido)){
                TempData["Mensaje"]="El Apellido no es valido";
                ModelState.AddModelError("UsuarioId.Apellido", "El Apellido no es valido.");
                return RedirectToAction(nameof(Create));
            }
            if (e.Id < 0 && e.UsuarioId.Telefono < 0)
            {
                TempData["Mensaje"]="El campo de Telefono es obligatorio";
                ModelState.AddModelError("UsuarioId.Telefono", "El campo Telefono es obligatorio.");
                return RedirectToAction(nameof(Create));
            }else if(!Expresiones.ValidarTelefono(e.UsuarioId.Telefono.ToString())){
                TempData["Mensaje"]="El Telefono no es valido";
                ModelState.AddModelError("UsuarioId.Telefono", "El Telefono no es valido.");
                return RedirectToAction(nameof(Create));
            }
            if (e.Id < 0 && string.IsNullOrEmpty(e.UsuarioId.Mail))
            {
                TempData["Mensaje"]="El campo de Mail es obligatorio";
                ModelState.AddModelError("UsuarioId.Mail", "El campo Mail es obligatorio.");
                return RedirectToAction(nameof(Create));
            }else if(e.Id < 0 && !Expresiones.ValidarMail(e.UsuarioId.Mail)){
                 TempData["Mensaje"]="El Mail es invalido, deberia estar en formato 'example@example.com'";
                ModelState.AddModelError("UsuarioId.Mail", "El Mail es invalido");
                return RedirectToAction(nameof(Create));
            }
            if(String.IsNullOrEmpty(e.Clave)){
                TempData["Mensaje"]="El campo Clave es obligatorio";
                ModelState.AddModelError("Clave", "El campo Clave es obligatorio");
                return RedirectToAction(nameof(Create));
            }else if(!Expresiones.ValidarClave(e.Clave)){
                TempData["Mensaje"]="La clave debe: \nIncluir al menos 8 caracteres.\nIncluir al menos una letra mayúscula.\nIncluir al menos un carácter especial (como !, @, #, etc.).";
                ModelState.AddModelError("Clave", "El campo Clave es obligatorio");
                return RedirectToAction(nameof(Create));
            }
            try
            {
                 
                var ER = new EmpleadosRepositorio();
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: e.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 250 / 8
                    ));
                e.Clave=hashed;
                ER.Alta(e);
                TempData["Id"] = e.Id;
                return RedirectToAction(nameof(Index));
            }
            catch(Exception E)
            {
                TempData["Mensaje"] = E.Message;
                Console.WriteLine(E.Message);
                return RedirectToAction(nameof(Create));
            }
        }

        // GET Empleados/Edit/5
        [Authorize]
        public ActionResult CambiarClave(int id)
        {
           var ER = new EmpleadosRepositorio();
            if (User.IsInRole("Empleado"))
			{
				if (User.Identity.Name != id+"")
                {
                    TempData["Mensaje"] = "No tienes permiso de realizar esta accion";
                    return RedirectToAction(nameof(Index), "Home"); 
                }
					
			}
            ViewBag.Id = TempData["Id"];
            if(TempData.ContainsKey("Mensaje"))
            {
                ViewBag.Mensaje = TempData["Mensaje"];

            }
            var E =ER.ObtenerXId(id);
            return View(E);
        }

        // POST Empleados/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CambiarClave(int id, Empleados e)
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
            if(String.IsNullOrEmpty(e.Clave)){
                TempData["Mensaje"]="El campo Clave es obligatorio";
                ModelState.AddModelError("Clave", "El campo Clave es obligatorio");
                return RedirectToAction(nameof(CambiarClave));
            }else if(!Expresiones.ValidarClave(e.Clave)){
                TempData["Mensaje"]="La clave debe: \nIncluir al menos 8 caracteres.\nIncluir al menos una letra mayúscula.\nIncluir al menos un carácter especial (como !, @, #, etc.).";
                ModelState.AddModelError("Clave", "El campo Clave es obligatorio");
                return RedirectToAction(nameof(CambiarClave));
            }
            try
            {  
                var ER = new EmpleadosRepositorio();
                
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: e.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 250 / 8
                    ));
                e.Clave=hashed;
                var bol =ER.CambiarClave(e);
                if(bol)
                {
                  TempData["Mensaje"]="Se modifico con exito la entidad id:"+e.Id;
                    return RedirectToAction(nameof(Index));  
                }else
                {
                    throw new Exception("No se pudo modificar con exito la entidad con id:"+e.Id);
                }
            }
            catch(Exception E)
            {
                TempData["Mensaje"] = E.Message;
                Console.WriteLine(E.Message);
                return RedirectToAction(nameof(CambiarClave));
            }
        }
        
        [Authorize]
        public ActionResult CambiarAvatar(int id)
        {
           var ER = new EmpleadosRepositorio();
            if (User.IsInRole("Empleado"))
			{
				if (User.Identity.Name != id+"")
                {
                    TempData["Mensaje"] = "No tienes permiso de realizar esta accion";
                    return RedirectToAction(nameof(Index), "Home"); 
                }
					
			}
            ViewBag.Id = TempData["Id"];
            if(TempData.ContainsKey("Mensaje"))
            {
                ViewBag.Mensaje = TempData["Mensaje"];

            }
            var A =ER.ObtenerXId(id);
            return View(A);
        }
        // POST Administradores/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CambiarAvatar(int id, Empleados a)
        {
            if (User.IsInRole("Empleado"))
                {
                    if (User.Identity.Name != id+"")
                    {
                        TempData["Mensaje"] = "No tienes permiso de realizar esta accion";
                        return RedirectToAction(nameof(Index), "Home"); 
                    }
                        
                }
            try
            {
                var ER = new EmpleadosRepositorio();
                
                var bol =ER.CambiarAvatar(a);
                if(bol)
                {
                  TempData["Mensaje"]="Se modifico el avatar con exito";
                    return RedirectToAction(nameof(Index));  
                }else
                {
                    throw new Exception("No se pudo modificar con exito la entidad con id:"+a.Id);
                }
            }
            catch(Exception e)
            {
                TempData["Mensaje"] = e.Message;
                Console.WriteLine(e.Message);
                return RedirectToAction(nameof(CambiarAvatar));
            }
        }

        // GET Empleados/Delete/5
        [Authorize]
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
            var ER = new EmpleadosRepositorio();
            var E = ER.ObtenerXId(id);    
            return View(E);
        }

        // POST Empleados/Delete/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Empleados e)
        {
            if (User.IsInRole("Empleado"))
            {
                TempData["Mensaje"] = "No tienes permiso de realizar esta accion";
                return RedirectToAction(nameof(Index), "Home"); 
            }
            try
            {
                 
                var ER = new EmpleadosRepositorio();
                var bol = ER.Baja(e);
                if(bol)
                {
                    TempData["Mensaje"] = "Se elimino con exito la entidad";
                    return RedirectToAction(nameof(Index));

                }else{
                    throw new Exception("No se logro eliminar la entidad id: "+id);
                }
            }
            catch(Exception E)
            {
                TempData["Mensaje"] = E.Message;
                Console.WriteLine(E.Message);
                return RedirectToAction(nameof(Delete));
            }
        }
    }
}