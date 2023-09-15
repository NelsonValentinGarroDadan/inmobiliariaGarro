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
            var IR = new InmueblesRepositorio();
            var U = IR.ObtenerTodos();
            return View(U);
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