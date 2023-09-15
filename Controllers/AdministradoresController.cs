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
    public class AdministradoresController : Controller
    {
         private readonly IConfiguration configuration;
		private readonly IWebHostEnvironment environment;

		public AdministradoresController(IConfiguration configuration, IWebHostEnvironment environment)
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
            var PR = new AdministradoresRepositorio();
            
            return View(PR.ObtenerTodos());
        }
        [Authorize]
        // GET Administradores/Details/5
        public ActionResult Details(int id)
        {
             if (User.IsInRole("Empleado"))
                {
                        TempData["Mensaje"] = "No tienes permiso de ver este contenido";
                        return RedirectToAction(nameof(Index), "Home"); 
            }
            var PR = new AdministradoresRepositorio();
            var A = PR.ObtenerXId(id);
            return View("../Usuarios/Details",A.UsuarioId);
        }
        [Authorize]
        public ActionResult Perfil(int id)
        {
             if (User.IsInRole("Empleado"))
                {
                        TempData["Mensaje"] = "No tienes permiso de ver este contenido";
                        return RedirectToAction(nameof(Index), "Home"); 
            }
            var PR = new AdministradoresRepositorio();
            var E = PR.ObtenerXId(id);
            return View(E);
        }
        [Authorize]
        // GET Administradores/Create
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
            var A = new Administradores();
            ViewBag.Usuarios = UR.ObtenerTodos();
            return View(A);
        }
        // POST Administradores/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create(Administradores a)
        {
            //ViewBag
            ViewBag.Id = a.Id;
            //Validaciones de Entrada 
            if (a.Id < 0)
            {
                TempData["Mensaje"]="Debes Elegir un Usuario";
                ModelState.AddModelError("Id", "Debes Elegir un Usuario");
                return RedirectToAction(nameof(Create));
            }
            if(String.IsNullOrEmpty(a.Clave)){
                TempData["Mensaje"]="El campo Clave es obligatorio";
                ModelState.AddModelError("Clave", "El campo Clave es obligatorio");
                return RedirectToAction(nameof(Create));
            }else if(!Expresiones.ValidarClave(a.Clave)){
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
                var AR = new AdministradoresRepositorio();
                 string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: a.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 250 / 8
                    ));
                a.Clave=hashed;
                AR.Alta(a);
                if(a.AvatarFileName != null && a.Id>0){
                    string wwwPath = environment.WebRootPath;
                    string path = Path.Combine(wwwPath, "Uploads");
                    if(!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string fileName = "avatar_" + a.Id + Path.GetExtension(a.AvatarFileName.FileName);
                    string pathCompleto = Path.Combine(path, fileName);
                    a.Avatar = Path.Combine("/Uploads", fileName);
                    using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                    {
                        a.AvatarFileName.CopyTo(stream);
                    }
                    AR.CambiarAvatar(a);
                }
                TempData["Id"] = a.Id;
                return RedirectToAction(nameof(Index));
            }
            catch(Exception e)
            {
                TempData["Mensaje"] = e.Message;
                Console.WriteLine(e);
                return RedirectToAction(nameof(Create));
            }
        }

        // GET Administradores/Edit/5
        [Authorize]
        public ActionResult CambiarClave(int id)
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
            var AR = new AdministradoresRepositorio(); 
            var A =AR.ObtenerXId(id);
            return View(A);
        }
        // POST Administradores/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CambiarClave(int id, Administradores a)
        {
             //Validaciones de Entrada 
            if(String.IsNullOrEmpty(a.Clave)){
                TempData["Mensaje"]="El campo Clave es obligatorio";
                ModelState.AddModelError("Clave", "El campo Clave es obligatorio");
                return RedirectToAction(nameof(CambiarClave));
            }else if(!Expresiones.ValidarClave(a.Clave)){
                TempData["Mensaje"]="La clave debe: \nIncluir al menos 8 caracteres.\nIncluir al menos una letra mayúscula.\nIncluir al menos un carácter especial (como !, @, #, etc.).";
                ModelState.AddModelError("Clave", "El campo Clave es obligatorio");
                return RedirectToAction(nameof(CambiarClave));
            }
            try
            {
                 if (User.IsInRole("Empleado"))
                {
                        TempData["Mensaje"] = "No tienes permiso de realizar esta accion";
                        return RedirectToAction(nameof(Index), "Home"); 
                }
                var AR = new AdministradoresRepositorio();
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: a.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 250 / 8
                    ));
                a.Clave=hashed;
                var bol =AR.CambiarClave(a);
                if(bol)
                {
                  TempData["Mensaje"]="Se modifico la contraseña con exito";
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
                return RedirectToAction(nameof(CambiarClave));
            }
        }
        
        [Authorize]
        public ActionResult CambiarAvatar(int id)
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
            var AR = new AdministradoresRepositorio(); 
            var A =AR.ObtenerXId(id);
            return View(A);
        }
        // POST Administradores/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CambiarAvatar(int id, Administradores a)
        {
            try
            {
                 if (User.IsInRole("Empleado"))
                {
                        TempData["Mensaje"] = "No tienes permiso de realizar esta accion";
                        return RedirectToAction(nameof(Index), "Home"); 
            }
                var AR = new AdministradoresRepositorio();
                var bol =AR.CambiarAvatar(a);
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

        // GET Administradores/Delete/5
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
            var AR = new AdministradoresRepositorio();
            var A = AR.ObtenerXId(id);    
            return View(A);
        }

        // POST Administradores/Delete/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Administradores a)
        {
            try
            {
                 if (User.IsInRole("Empleado"))
                {
                        TempData["Mensaje"] = "No tienes permiso de realizar esta accion";
                        return RedirectToAction(nameof(Index), "Home"); 
            }
                var AR = new AdministradoresRepositorio();
                var bol = AR.Baja(a);
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
    }
}