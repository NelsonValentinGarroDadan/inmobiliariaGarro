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
    public class PropietariosController : Controller
    {
        private readonly IConfiguration configuration;
		private readonly IWebHostEnvironment environment;

		public PropietariosController(IConfiguration configuration, IWebHostEnvironment environment)
		{
			this.configuration = configuration;
			this.environment = environment;
		}
        
        // GET: Usuarios
        [Authorize]
        public ActionResult Index()
        {
             
             ViewBag.Id = TempData["Id"];
            if(TempData.ContainsKey("Mensaje"))
            {
                ViewBag.Mensaje = TempData["Mensaje"];

            }
            var PR = new PropietariosRepositorio();
            var P = PR.ObtenerTodos();
            return View(P);
        }
        [Authorize]
        public ActionResult Details(int id)
        {
            var PR = new PropietariosRepositorio();
            var P = PR.ObtenerXId(id);
            return View(P);
        }
        [Authorize]
        public ActionResult Create()
        { 
            ViewBag.Id = TempData["Id"];
            if(TempData.ContainsKey("Mensaje"))
            {
                ViewBag.Mensaje = TempData["Mensaje"];

            }
            var UR = new UsuariosRepositorio();
            var P = new Propietarios();
            ViewBag.Usuarios = UR.ObtenerTodos();
            return View(P);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Propietarios p)
        { 
            //Validaciones de Entrada 
            if(p.Id==0 && String.IsNullOrEmpty(p.UsuarioId.DNI) && String.IsNullOrEmpty(p.UsuarioId.Nombre) && String.IsNullOrEmpty(p.UsuarioId.Apellido) && p.UsuarioId.Telefono == 0 && String.IsNullOrEmpty(p.UsuarioId.Mail))
            {
                TempData["Mensaje"]="Debes Elegir un Usuario o crear uno";
                return RedirectToAction(nameof(Create)); 
            }
            if(p.Id != 0 && (!String.IsNullOrEmpty(p.UsuarioId.DNI)|| !String.IsNullOrEmpty(p.UsuarioId.Nombre) || !String.IsNullOrEmpty(p.UsuarioId.Apellido) || p.UsuarioId.Telefono != 0 || !String.IsNullOrEmpty(p.UsuarioId.Mail)))
            {
               TempData["Mensaje"]="Debes Elegir un Usuario o crear uno, no puede hacer ambos";
                return RedirectToAction(nameof(Create)); 
            }
            if(p.Id == 0 )
            {
                if (String.IsNullOrEmpty(p.UsuarioId.DNI))
                {
                    TempData["Mensaje"]="El campo de DNI es obligatorio";
                    ModelState.AddModelError("UsuarioId.DNI", "El campo DNI es obligatorio.");
                    return RedirectToAction(nameof(Create));
                }else if(!Expresiones.ValidarDNI(p.UsuarioId.DNI.ToString())){
                    TempData["Mensaje"]="El DNI no es valido, se permiten solo 7 o 8 caracteres";
                    ModelState.AddModelError("UsuarioId.DNI", "El DNI no es valido, se permiten solo 7 o 8 caracteres.");
                    return RedirectToAction(nameof(Create));
                }
                if (string.IsNullOrEmpty(p.UsuarioId.Nombre))
                {
                    TempData["Mensaje"]="El campo de Nombre es obligatorio";
                    ModelState.AddModelError("UsuarioId.Nombre", "El campo Nombre es obligatorio.");
                    return RedirectToAction(nameof(Create));
                }else if(!Expresiones.ValidarNombre(p.UsuarioId.Nombre)){
                    TempData["Mensaje"]="El Nombre es valido";
                    ModelState.AddModelError("UsuarioId.Nombre", "El Nombre es valido");
                    return RedirectToAction(nameof(Create));
                }
                if (string.IsNullOrEmpty(p.UsuarioId.Apellido))
                {
                    TempData["Mensaje"]="El campo de Apellido es obligatorio";
                    ModelState.AddModelError("UsuarioId.Apellido", "El campo Apellido es obligatorio.");
                    return RedirectToAction(nameof(Create));
                } else if(!Expresiones.ValidarApellido(p.UsuarioId.Apellido)){
                    TempData["Mensaje"]="El Apellido no es valido";
                    ModelState.AddModelError("UsuarioId.Apellido", "El Apellido no es valido.");
                    return RedirectToAction(nameof(Create));
                }
                if (p.UsuarioId.Telefono < 0)
                {
                    TempData["Mensaje"]="El campo de Telefono es obligatorio";
                    ModelState.AddModelError("UsuarioId.Telefono", "El campo Telefono es obligatorio.");
                    return RedirectToAction(nameof(Create));
                }else if(!Expresiones.ValidarTelefono(p.UsuarioId.Telefono.ToString())){
                    TempData["Mensaje"]="El Telefono no es valido";
                    ModelState.AddModelError("UsuarioId.Telefono", "El Telefono no es valido.");
                    return RedirectToAction(nameof(Create));
                }
                if (string.IsNullOrEmpty(p.UsuarioId.Mail))
                {
                    TempData["Mensaje"]="El campo de Mail es obligatorio";
                    ModelState.AddModelError("UsuarioId.Mail", "El campo Mail es obligatorio.");
                    return RedirectToAction(nameof(Create));
                }else if(!Expresiones.ValidarMail(p.UsuarioId.Mail)){
                    TempData["Mensaje"]="El Mail es invalido, deberia estar en formato 'example@example.com'";
                    ModelState.AddModelError("UsuarioId.Mail", "El Mail es invalido");
                    return RedirectToAction(nameof(Create));
                }
            }
            try
            {
                var UR = new UsuariosRepositorio();
                var PR = new PropietariosRepositorio();
                p.UsuarioId.Id = p.Id;
                if(UR.Existe(p.UsuarioId)){
                    PR.Alta(p);
                    TempData["Mensaje"] = "Se creo con exito el inquilino y se asocio al usuario con id: "+p.UsuarioId.Id;
                }else{
                    p.UsuarioId.Nombre = char.ToUpper(p.UsuarioId.Nombre[0]) + p.UsuarioId.Nombre.Substring(1).ToLower();
                    p.UsuarioId.Apellido = char.ToUpper(p.UsuarioId.Apellido[0]) + p.UsuarioId.Apellido.Substring(1).ToLower();
                    p.Id = UR.Alta(p.UsuarioId);
                    PR.Alta(p);
                    TempData["Mensaje"] = "Se creo con exito el inquilino y el usuario nuevo con id: "+p.Id;
                    
                }
                
                return RedirectToAction(nameof(Index));
            }
            catch(Exception E)
            {
                TempData["Mensaje"] = E.Message;
                Console.WriteLine(E.Message);
                return RedirectToAction(nameof(Create));
            }
        } 
         [Authorize]
        // GET: Usuarios/Edit/5
        public ActionResult Edit(int id)
        { 
            var PR = new PropietariosRepositorio();
            
            ViewBag.Id = TempData["Id"];
            if(TempData.ContainsKey("Mensaje"))
            {
                ViewBag.Mensaje = TempData["Mensaje"];

            }
            return View(PR.ObtenerXId(id));
        }
        [Authorize]
        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Propietarios p)
        {
             //Validaciones de Entrada 
            if (string.IsNullOrEmpty(p.UsuarioId.DNI))
            {
                TempData["Mensaje"]="El campo de DNI es obligatorio";
                ModelState.AddModelError("UsuarioId.DNI", "El campo DNI es obligatorio.");
                return RedirectToAction(nameof(Create));
            }else if( !Expresiones.ValidarDNI(p.UsuarioId.DNI.ToString())){
                TempData["Mensaje"]="El DNI no es valido, se permiten solo 7 o 8 caracteres";
                ModelState.AddModelError("UsuarioId.DNI", "El DNI no es valido, se permiten solo 7 o 8 caracteres.");
                return RedirectToAction(nameof(Create));
            }
            if (string.IsNullOrEmpty(p.UsuarioId.Nombre))
            {
                TempData["Mensaje"]="El campo de Nombre es obligatorio";
                ModelState.AddModelError("UsuarioId.Nombre", "El campo Nombre es obligatorio.");
                return RedirectToAction(nameof(Create));
            }else if( !Expresiones.ValidarNombre(p.UsuarioId.Nombre)){
                TempData["Mensaje"]="El Nombre es valido";
                ModelState.AddModelError("UsuarioId.Nombre", "El Nombre es valido");
                return RedirectToAction(nameof(Create));
            }
            if (string.IsNullOrEmpty(p.UsuarioId.Apellido))
            {
                TempData["Mensaje"]="El campo de Apellido es obligatorio";
                ModelState.AddModelError("UsuarioId.Apellido", "El campo Apellido es obligatorio.");
                return RedirectToAction(nameof(Create));
            } else if(!Expresiones.ValidarApellido(p.UsuarioId.Apellido)){
                TempData["Mensaje"]="El Apellido no es valido";
                ModelState.AddModelError("UsuarioId.Apellido", "El Apellido no es valido.");
                return RedirectToAction(nameof(Create));
            }
            if (p.UsuarioId.Telefono < 0)
            {
                TempData["Mensaje"]="El campo de Telefono es obligatorio";
                ModelState.AddModelError("UsuarioId.Telefono", "El campo Telefono es obligatorio.");
                return RedirectToAction(nameof(Create));
            }else if(!Expresiones.ValidarTelefono(p.UsuarioId.Telefono.ToString())){
                TempData["Mensaje"]="El Telefono no es valido";
                ModelState.AddModelError("UsuarioId.Telefono", "El Telefono no es valido.");
                return RedirectToAction(nameof(Create));
            }
            try
            {   
                var UR = new UsuariosRepositorio();
                //Primera Mayuscula Nombre y Apelldo
                p.UsuarioId.Nombre = char.ToUpper(p.UsuarioId.Nombre[0]) + p.UsuarioId.Nombre.Substring(1).ToLower();
                p.UsuarioId.Apellido = char.ToUpper(p.UsuarioId.Apellido[0]) + p.UsuarioId.Apellido.Substring(1).ToLower();
                p.UsuarioId.Id = p.Id;
                var bol =UR.Modificacion(id,p.UsuarioId);
                if(bol)
                {
                  TempData["Mensaje"]="Se modifico con exito la entidad id:"+p.Id;
                return RedirectToAction(nameof(Index));  
                }else
                {
                    throw new Exception("No se pudo modificar con exito la entidad con id:"+p.Id);
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
            var PR = new PropietariosRepositorio();
            var P = PR.ObtenerXId(id);    
            return View(P);
        }

        // POST Empleados/Delete/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Propietarios e)
        {
            try
            {
                 if (User.IsInRole("Empleado"))
                {
                        TempData["Mensaje"] = "No tienes permiso de realizar esta accion";
                        return RedirectToAction(nameof(Index), "Home"); 
            }
                var PR = new PropietariosRepositorio();
                var bol = PR.Baja(e);
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