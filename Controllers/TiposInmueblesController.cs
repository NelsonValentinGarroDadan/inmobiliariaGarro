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
    public class TiposInmueblesController : Controller
    {
        // GET: TiposInmuebles
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
            var TER = new TiposInmueblesRepositorio();
            return View(TER.ObtenerTodos());
        }

        // GET: TiposInmuebles/Details/5
        [Authorize]
        public ActionResult Details(int id)
        {
             if (User.IsInRole("Empleado"))
                {
                        TempData["Mensaje"] = "No tienes permiso de realizar esta accion";
                        return RedirectToAction(nameof(Index), "Home"); 
            }
            var TER = new TiposInmueblesRepositorio();
            return View(TER.ObtenerXId(id));
        }

        // GET: TiposInmuebles/Create
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
            var TE = new TiposInmuebles();
            return View(TE);
        }

        // POST: TiposInmuebles/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TiposInmuebles te)
        {
            try
            {
                 if (User.IsInRole("Empleado"))
                {
                        TempData["Mensaje"] = "No tienes permiso de realizar esta accion";
                        return RedirectToAction(nameof(Index), "Home"); 
            }
                // TODO: Add insert logic here
                var TER = new TiposInmueblesRepositorio();
                TER.Alta(te);
                
                TempData["Id"] = te.Id;

                return RedirectToAction(nameof(Index));
            }
            catch(Exception e)
            {
                TempData["Mensaje"] = e.Message;
                Console.WriteLine(e.Message);
                return RedirectToAction(nameof(Create));
            }
        }

        // GET: TiposInmuebles/Edit/5
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
            var TER = new TiposInmueblesRepositorio();
            
            return View(TER.ObtenerXId(id));
        }

        // POST: TiposInmuebles/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, TiposInmuebles te)
        {
            try
            {
                 if (User.IsInRole("Empleado"))
                {
                        TempData["Mensaje"] = "No tienes permiso de realizar esta accion";
                        return RedirectToAction(nameof(Index), "Home"); 
            }
                // TODO: Add update logic here
                var TER = new TiposInmueblesRepositorio();
                var bol =TER.Modificacion(te);
                if(bol)
                {
                  TempData["Mensaje"]="Se modifico con exito la entidad id:"+te.Id;
                return RedirectToAction(nameof(Index));  
                }else
                {
                    throw new Exception("No se pudo modificar con exito la entidad con id:"+te.Id);
                }

                
            }
            catch(Exception e)
            {
                TempData["Mensaje"] = e.Message;
                Console.WriteLine(e.Message);
                return RedirectToAction(nameof(Edit));
            }
        }

        // GET: TiposInmuebles/Delete/5
        [Authorize]
        public ActionResult Delete(int id){
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
            var TER = new TiposInmueblesRepositorio();
            
            return View(TER.ObtenerXId(id));
        }

        // POST: TiposInmuebles/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Delete(int id, TiposInmuebles te)
        {
            try
            {
                 if (User.IsInRole("Empleado"))
                {
                        TempData["Mensaje"] = "No tienes permiso de realizar esta accion";
                        return RedirectToAction(nameof(Index), "Home"); 
            }
                var TER = new TiposInmueblesRepositorio();
                var bol = TER.Baja(te);
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