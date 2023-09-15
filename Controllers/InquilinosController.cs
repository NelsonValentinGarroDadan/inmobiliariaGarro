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
            var CR = new ContratosRepositorio();
            var C = CR.ObtenerTodos();
            return View(C);
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
        public ActionResult Delete(int id, Inquilinos e)
        {
            try
            {
                 if (User.IsInRole("Empleado"))
                {
                        TempData["Mensaje"] = "No tienes permiso de realizar esta accion";
                        return RedirectToAction(nameof(Index), "Home"); 
            }
                var IR = new InquilinosRepositorio();
                var bol = IR.Baja(e);
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