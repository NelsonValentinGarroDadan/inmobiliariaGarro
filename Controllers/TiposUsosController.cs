using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;
using inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliaria.Controllers
{
    public class TiposUsosController : Controller
    {
        // GET: TiposUsos
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
            var TUR = new TiposUsosRepositorio();
            return View(TUR.ObtenerTodos());
        }

        // GET: TiposUsos/Details/5
        [Authorize]
        public ActionResult Details(int id)
        {
             if (User.IsInRole("Empleado"))
                {
                        TempData["Mensaje"] = "No tienes permiso de realizar esta accion";
                        return RedirectToAction(nameof(Index), "Home"); 
            }
            var TUR = new TiposUsosRepositorio();
            return View(TUR.ObtenerXId(id));
        }

        // GET: TiposUsos/Create
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
            var TU = new TiposUsos();
            return View(TU);
        }

        // POST: TiposUsos/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TiposUsos tu)
        {
            try
            {
                 if (User.IsInRole("Empleado"))
                {
                        TempData["Mensaje"] = "No tienes permiso de realizar esta accion";
                        return RedirectToAction(nameof(Index), "Home"); 
            }
                // TODO: Add insert logic here
                var TUR = new TiposUsosRepositorio();
                TUR.Alta(tu);
                
                TempData["Id"] = tu.Id;
                Console.WriteLine(tu.Id);

                return RedirectToAction(nameof(Index));
            }
            catch(Exception e)
            {
                TempData["Mensaje"] = e.Message;
                Console.WriteLine(e.Message);
                return RedirectToAction(nameof(Create));
            }
        }

        // GET: TiposUsos/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
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
            var TUR = new TiposUsosRepositorio();
            
            return View(TUR.ObtenerXId(id));
        }

        // POST: TiposUsos/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, TiposUsos tu)
        {
            try
            {
                 if (User.IsInRole("Empleado"))
                {
                        TempData["Mensaje"] = "No tienes permiso de realizar esta accion";
                        return RedirectToAction(nameof(Index), "Home"); 
            }
                // TODO: Add update logic here
                var TUR = new TiposUsosRepositorio();
                var bol =TUR.Modificacion(tu);
                if(bol)
                {
                  TempData["Mensaje"]="Se modifico con exito la entidad id:"+tu.Id;
                return RedirectToAction(nameof(Index));  
                }else
                {
                    throw new Exception("No se pudo moficar con exito la entidad con id:"+tu.Id);
                }

                
            }
            catch(Exception e)
            {
                TempData["Mensaje"] = e.Message;
                Console.WriteLine(e.Message);
                return RedirectToAction(nameof(Edit));
            }
        }

        // GET: TiposUsos/Delete/5
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
            var TUR = new TiposUsosRepositorio();
            
            return View(TUR.ObtenerXId(id));
        }

        // POST: TiposUsos/Delete/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, TiposUsos tu)
        {
            try
            {
                 if (User.IsInRole("Empleado"))
                {
                        TempData["Mensaje"] = "No tienes permiso de realizar esta accion";
                        return RedirectToAction(nameof(Index), "Home"); 
            }
                var TUR = new TiposUsosRepositorio();
                var bol = TUR.Baja(tu);
                TempData["Mensaje"] = "Se elimino con exito la entidad";
                return RedirectToAction(nameof(Index));

                
            }
            catch(Exception e)
            {
                TempData["Mensaje"] = "No puedes eliminar este Tipo de Uso porque esta asociado a un Inmueble";
                Console.WriteLine(e.Message);
                return RedirectToAction(nameof(Delete));
            }
        }
    }
}