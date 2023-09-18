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
            if (User.IsInRole("Empleado"))
                {
                        TempData["Mensaje"] = "No tienes permiso de realizar esta accion";
                        return RedirectToAction(nameof(Index), "Home"); 
            }
             //Validaciones de Entrada 
             
            if(a.Id==0 && String.IsNullOrEmpty(a.UsuarioId.DNI) && String.IsNullOrEmpty(a.UsuarioId.Nombre) && String.IsNullOrEmpty(a.UsuarioId.Apellido) && a.UsuarioId.Telefono == 0 && String.IsNullOrEmpty(a.UsuarioId.Mail))
            {
                TempData["Mensaje"]="Debes Elegir un Usuario o crear uno";
                return RedirectToAction(nameof(Create)); 
            }
            if(a.Id != 0 && (!String.IsNullOrEmpty(a.UsuarioId.DNI)|| !String.IsNullOrEmpty(a.UsuarioId.Nombre) || !String.IsNullOrEmpty(a.UsuarioId.Apellido) || a.UsuarioId.Telefono != 0 || !String.IsNullOrEmpty(a.UsuarioId.Mail)))
            {
               TempData["Mensaje"]="Debes Elegir un Usuario o crear uno, no puede hacer ambos";
                return RedirectToAction(nameof(Create)); 
            }
            if(a.Id == 0 )
            {
                if (String.IsNullOrEmpty(a.UsuarioId.DNI))
                {
                    TempData["Mensaje"]="El campo de DNI es obligatorio";
                    ModelState.AddModelError("UsuarioId.DNI", "El campo DNI es obligatorio.");
                    return RedirectToAction(nameof(Create));
                }else if(!Expresiones.ValidarDNI(a.UsuarioId.DNI.ToString())){
                    TempData["Mensaje"]="El DNI no es valido, se permiten solo 7 o 8 caracteres";
                    ModelState.AddModelError("UsuarioId.DNI", "El DNI no es valido, se permiten solo 7 o 8 caracteres.");
                    return RedirectToAction(nameof(Create));
                }
                if (string.IsNullOrEmpty(a.UsuarioId.Nombre))
                {
                    TempData["Mensaje"]="El campo de Nombre es obligatorio";
                    ModelState.AddModelError("UsuarioId.Nombre", "El campo Nombre es obligatorio.");
                    return RedirectToAction(nameof(Create));
                }else if(!Expresiones.ValidarNombre(a.UsuarioId.Nombre)){
                    TempData["Mensaje"]="El Nombre es valido";
                    ModelState.AddModelError("UsuarioId.Nombre", "El Nombre es valido");
                    return RedirectToAction(nameof(Create));
                }
                if (string.IsNullOrEmpty(a.UsuarioId.Apellido))
                {
                    TempData["Mensaje"]="El campo de Apellido es obligatorio";
                    ModelState.AddModelError("UsuarioId.Apellido", "El campo Apellido es obligatorio.");
                    return RedirectToAction(nameof(Create));
                } else if(!Expresiones.ValidarApellido(a.UsuarioId.Apellido)){
                    TempData["Mensaje"]="El Apellido no es valido";
                    ModelState.AddModelError("UsuarioId.Apellido", "El Apellido no es valido.");
                    return RedirectToAction(nameof(Create));
                }
                if (a.UsuarioId.Telefono < 0)
                {
                    TempData["Mensaje"]="El campo de Telefono es obligatorio";
                    ModelState.AddModelError("UsuarioId.Telefono", "El campo Telefono es obligatorio.");
                    return RedirectToAction(nameof(Create));
                }else if(!Expresiones.ValidarTelefono(a.UsuarioId.Telefono.ToString())){
                    TempData["Mensaje"]="El Telefono no es valido";
                    ModelState.AddModelError("UsuarioId.Telefono", "El Telefono no es valido.");
                    return RedirectToAction(nameof(Create));
                }
                if (string.IsNullOrEmpty(a.UsuarioId.Mail))
                {
                    TempData["Mensaje"]="El campo de Mail es obligatorio";
                    ModelState.AddModelError("UsuarioId.Mail", "El campo Mail es obligatorio.");
                    return RedirectToAction(nameof(Create));
                }else if(!Expresiones.ValidarMail(a.UsuarioId.Mail)){
                    TempData["Mensaje"]="El Mail es invalido, deberia estar en formato 'example@example.com'";
                    ModelState.AddModelError("UsuarioId.Mail", "El Mail es invalido");
                    return RedirectToAction(nameof(Create));
                }
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
                var UR = new UsuariosRepositorio();
                var AR = new AdministradoresRepositorio();
                 string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: a.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 250 / 8
                    ));
                a.Clave=hashed;
                if(UR.Existe(a.UsuarioId)){
                    AR.Alta(a);
                    TempData["Mensaje"] = "Se creo con exito el administrador y se asocio al usuario con id: "+a.Id;
                }else{
                    a.Id = UR.Alta(a.UsuarioId);
                    AR.Alta(a);
                    TempData["Mensaje"] = "Se creo con exito el administrador y el usuario nuevo con id: "+a.Id;
                }
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
                    
                }
                var bol=AR.CambiarAvatar(a);
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