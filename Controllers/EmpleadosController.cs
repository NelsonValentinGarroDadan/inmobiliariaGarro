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
                        TempData["Mensaje"] = "No tienes permiso de realizar esta accion";
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
           
            var UR = new UsuariosRepositorio();
            if (User.IsInRole("Empleado"))
			{
				var user = UR.ObtenerXMail(User.Identity.Name);
				if (user.Id != id)
                {
                    TempData["Mensaje"] = "No tienes permiso de ver";
                    return RedirectToAction(nameof(Index), "Home"); 
                }
					
			}
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
            ViewBag.Id = TempData["Id"];
            if(TempData.ContainsKey("Mensaje"))
            {
                ViewBag.Mensaje = TempData["Mensaje"];

            }
            if (User.IsInRole("Empleado"))
                {
                        TempData["Mensaje"] = "No tienes permiso de realizar esta accion";
                        return RedirectToAction(nameof(Index), "Home"); 
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
            //ViewBag
            ViewBag.Id = e.Id;
            //Validaciones de Entrada 
            if (e.Id < 0)
            {
                TempData["Mensaje"]="Debes Elegir un Usuario";
                ModelState.AddModelError("Id", "Debes Elegir un Usuario");
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
                 if (User.IsInRole("Empleado"))
                {
                        TempData["Mensaje"] = "No tienes permiso de realizar esta accion";
                        return RedirectToAction(nameof(Index), "Home"); 
            }
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
                if (User.IsInRole("Empleado"))
                {
                    if (User.Identity.Name != id+"")
                    {
                        TempData["Mensaje"] = "No tienes permiso de realizar esta accion";
                        return RedirectToAction(nameof(Index), "Home"); 
                    }
                        
                }
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
            try
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
            try
            {
                 if (User.IsInRole("Empleado"))
                {
                        TempData["Mensaje"] = "No tienes permiso de realizar esta accion";
                        return RedirectToAction(nameof(Index), "Home"); 
            }
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