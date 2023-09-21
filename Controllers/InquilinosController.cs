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
    public class InquilinosController : Controller
    {
        private readonly IConfiguration configuration;
		private readonly IWebHostEnvironment environment;

		public InquilinosController(IConfiguration configuration, IWebHostEnvironment environment)
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
            var IR = new InquilinosRepositorio();
            var I = IR.ObtenerTodos();
            return View(I);
        }
        [Authorize]
        public ActionResult Details(int id)
        {
            var IR = new InquilinosRepositorio();
            var I = IR.ObtenerXId(id);
            return View(I);
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
            var I = new Inquilinos();
            ViewBag.Usuarios = UR.ObtenerTodos();
            return View(I);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Inquilinos i)
        { //Validaciones de Entrada 
            //Validaciones de Entrada 
            if(i.Id==0 && String.IsNullOrEmpty(i.UsuarioId.DNI) && String.IsNullOrEmpty(i.UsuarioId.Nombre) && String.IsNullOrEmpty(i.UsuarioId.Apellido) && i.UsuarioId.Telefono == 0 && String.IsNullOrEmpty(i.UsuarioId.Mail))
            {
                TempData["Mensaje"]="Debes Elegir un Usuario o crear uno";
                return RedirectToAction(nameof(Create)); 
            }
            if(i.Id != 0 && (!String.IsNullOrEmpty(i.UsuarioId.DNI)|| !String.IsNullOrEmpty(i.UsuarioId.Nombre) || !String.IsNullOrEmpty(i.UsuarioId.Apellido) || i.UsuarioId.Telefono != 0 || !String.IsNullOrEmpty(i.UsuarioId.Mail)))
            {
               TempData["Mensaje"]="Debes Elegir un Usuario o crear uno, no puede hacer ambos";
                return RedirectToAction(nameof(Create)); 
            }
            if(i.Id == 0 )
            {
                if (String.IsNullOrEmpty(i.UsuarioId.DNI))
                {
                    TempData["Mensaje"]="El campo de DNI es obligatorio";
                    ModelState.AddModelError("UsuarioId.DNI", "El campo DNI es obligatorio.");
                    return RedirectToAction(nameof(Create));
                }else if(!Expresiones.ValidarDNI(i.UsuarioId.DNI.ToString())){
                    TempData["Mensaje"]="El DNI no es valido, se permiten solo 7 o 8 caracteres";
                    ModelState.AddModelError("UsuarioId.DNI", "El DNI no es valido, se permiten solo 7 o 8 caracteres.");
                    return RedirectToAction(nameof(Create));
                }
                if (string.IsNullOrEmpty(i.UsuarioId.Nombre))
                {
                    TempData["Mensaje"]="El campo de Nombre es obligatorio";
                    ModelState.AddModelError("UsuarioId.Nombre", "El campo Nombre es obligatorio.");
                    return RedirectToAction(nameof(Create));
                }else if(!Expresiones.ValidarNombre(i.UsuarioId.Nombre)){
                    TempData["Mensaje"]="El Nombre es valido";
                    ModelState.AddModelError("UsuarioId.Nombre", "El Nombre es valido");
                    return RedirectToAction(nameof(Create));
                }
                if (string.IsNullOrEmpty(i.UsuarioId.Apellido))
                {
                    TempData["Mensaje"]="El campo de Apellido es obligatorio";
                    ModelState.AddModelError("UsuarioId.Apellido", "El campo Apellido es obligatorio.");
                    return RedirectToAction(nameof(Create));
                } else if(!Expresiones.ValidarApellido(i.UsuarioId.Apellido)){
                    TempData["Mensaje"]="El Apellido no es valido";
                    ModelState.AddModelError("UsuarioId.Apellido", "El Apellido no es valido.");
                    return RedirectToAction(nameof(Create));
                }
                if (i.UsuarioId.Telefono < 0)
                {
                    TempData["Mensaje"]="El campo de Telefono es obligatorio";
                    ModelState.AddModelError("UsuarioId.Telefono", "El campo Telefono es obligatorio.");
                    return RedirectToAction(nameof(Create));
                }else if(!Expresiones.ValidarTelefono(i.UsuarioId.Telefono.ToString())){
                    TempData["Mensaje"]="El Telefono no es valido";
                    ModelState.AddModelError("UsuarioId.Telefono", "El Telefono no es valido.");
                    return RedirectToAction(nameof(Create));
                }
                if (string.IsNullOrEmpty(i.UsuarioId.Mail))
                {
                    TempData["Mensaje"]="El campo de Mail es obligatorio";
                    ModelState.AddModelError("UsuarioId.Mail", "El campo Mail es obligatorio.");
                    return RedirectToAction(nameof(Create));
                }else if(!Expresiones.ValidarMail(i.UsuarioId.Mail)){
                    TempData["Mensaje"]="El Mail es invalido, deberia estar en formato 'example@example.com'";
                    ModelState.AddModelError("UsuarioId.Mail", "El Mail es invalido");
                    return RedirectToAction(nameof(Create));
                }
            }
            try
            {
                var UR = new UsuariosRepositorio();
                var IR = new InquilinosRepositorio();
                i.UsuarioId.Id = i.Id;
                if(UR.Existe(i.UsuarioId)){
                    IR.Alta(i);
                    TempData["Mensaje"] = "Se creo con exito el inquilino y se asocio al usuario con id: "+i.UsuarioId.Id;
                }else{
                    i.UsuarioId.Nombre = char.ToUpper(i.UsuarioId.Nombre[0]) + i.UsuarioId.Nombre.Substring(1).ToLower();
                    i.UsuarioId.Apellido = char.ToUpper(i.UsuarioId.Apellido[0]) + i.UsuarioId.Apellido.Substring(1).ToLower();
                    i.Id = UR.Alta(i.UsuarioId);
                    IR.Alta(i);
                    TempData["Mensaje"] = "Se creo con exito el inquilino y el usuario nuevo con id: "+i.UsuarioId.Id;
                    
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
            var IR = new InquilinosRepositorio();
            
            ViewBag.Id = TempData["Id"];
            if(TempData.ContainsKey("Mensaje"))
            {
                ViewBag.Mensaje = TempData["Mensaje"];

            }
            return View(IR.ObtenerXId(id));
        }
        [Authorize]
        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Inquilinos i)
        {
             //Validaciones de Entrada 
            if (string.IsNullOrEmpty(i.UsuarioId.DNI))
            {
                TempData["Mensaje"]="El campo de DNI es obligatorio";
                ModelState.AddModelError("UsuarioId.DNI", "El campo DNI es obligatorio.");
                return RedirectToAction(nameof(Create));
            }else if( !Expresiones.ValidarDNI(i.UsuarioId.DNI.ToString())){
                TempData["Mensaje"]="El DNI no es valido, se permiten solo 7 o 8 caracteres";
                ModelState.AddModelError("UsuarioId.DNI", "El DNI no es valido, se permiten solo 7 o 8 caracteres.");
                return RedirectToAction(nameof(Create));
            }
            if (string.IsNullOrEmpty(i.UsuarioId.Nombre))
            {
                TempData["Mensaje"]="El campo de Nombre es obligatorio";
                ModelState.AddModelError("UsuarioId.Nombre", "El campo Nombre es obligatorio.");
                return RedirectToAction(nameof(Create));
            }else if( !Expresiones.ValidarNombre(i.UsuarioId.Nombre)){
                TempData["Mensaje"]="El Nombre es valido";
                ModelState.AddModelError("UsuarioId.Nombre", "El Nombre es valido");
                return RedirectToAction(nameof(Create));
            }
            if (string.IsNullOrEmpty(i.UsuarioId.Apellido))
            {
                TempData["Mensaje"]="El campo de Apellido es obligatorio";
                ModelState.AddModelError("UsuarioId.Apellido", "El campo Apellido es obligatorio.");
                return RedirectToAction(nameof(Create));
            } else if(!Expresiones.ValidarApellido(i.UsuarioId.Apellido)){
                TempData["Mensaje"]="El Apellido no es valido";
                ModelState.AddModelError("UsuarioId.Apellido", "El Apellido no es valido.");
                return RedirectToAction(nameof(Create));
            }
            if (i.UsuarioId.Telefono < 0)
            {
                TempData["Mensaje"]="El campo de Telefono es obligatorio";
                ModelState.AddModelError("UsuarioId.Telefono", "El campo Telefono es obligatorio.");
                return RedirectToAction(nameof(Create));
            }else if(!Expresiones.ValidarTelefono(i.UsuarioId.Telefono.ToString())){
                TempData["Mensaje"]="El Telefono no es valido";
                ModelState.AddModelError("UsuarioId.Telefono", "El Telefono no es valido.");
                return RedirectToAction(nameof(Create));
            }
            try
            {   
                var UR = new UsuariosRepositorio();
                var IR = new InquilinosRepositorio();
                //Primera Mayuscula Nombre y Apelldo
                i.UsuarioId.Nombre = char.ToUpper(i.UsuarioId.Nombre[0]) + i.UsuarioId.Nombre.Substring(1).ToLower();
                i.UsuarioId.Apellido = char.ToUpper(i.UsuarioId.Apellido[0]) + i.UsuarioId.Apellido.Substring(1).ToLower();
                i.UsuarioId.Id = i.Id;
                var bol =UR.Modificacion(id,i.UsuarioId);
                if(bol)
                {
                  TempData["Mensaje"]="Se modifico con exito la entidad id:"+i.Id;
                return RedirectToAction(nameof(Index));  
                }else
                {
                    throw new Exception("No se pudo modificar con exito la entidad con id:"+i.Id);
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
            var IR = new InquilinosRepositorio();
            var I = IR.ObtenerXId(id);    
            return View(I);
        }

        // POST Empleados/Delete/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Inquilinos i)
        {
            try
            {
                 if (User.IsInRole("Empleado"))
                {
                        TempData["Mensaje"] = "No tienes permiso de realizar esta accion";
                        return RedirectToAction(nameof(Index), "Home"); 
            }
                var IR = new InquilinosRepositorio();
                var bol = IR.Baja(i);
                TempData["Mensaje"] = "Se elimino con exito la entidad";
                return RedirectToAction(nameof(Index));
            }
            catch(Exception E)
            {
                TempData["Mensaje"] = "No puedes eliminar este Inquilino porque esta asociado a un Contrato";
                Console.WriteLine(E.Message);
                return RedirectToAction(nameof(Delete));
            }
        }
   }    
}