using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliaria.Controllers
{
    public class PagosController : Controller
    {
        // GET: Pagos
        [Authorize]
        public ActionResult Index(int filtroContrato)
        {
            ViewBag.filtroContrato  = filtroContrato;
            
            ViewBag.Id = TempData["Id"];
            if(TempData.ContainsKey("Mensaje"))
            {
                ViewBag.Mensaje = TempData["Mensaje"];

            }
            var CR = new ContratosRepositorio();
            ViewBag.Contratos = CR.ObtenerTodos();
            var PR = new PagosRepositorio();
            //Logica de filtrado
            var todosP = PR.ObtenerTodos();
            List<Pagos> P = new List<Pagos>();
            List<bool> condiciones ;
            bool condicionFinal;
            for(int i = 0; i < todosP.Count ; i++){
                condicionFinal = true;
                condiciones = new List<bool>();
                if( filtroContrato != 0 ) condiciones.Add(filtroContrato == todosP[i].ContratoId.Id ); 
                for(int j = 0; j<condiciones.Count;j++) condicionFinal= condicionFinal && condiciones[j];

                if(condicionFinal) P.Add(todosP[i]);
            }
            return View(P);
        }

        // GET: Pagos/Details/5
        [Authorize]
        public ActionResult Details(int id)
        {
           
            var PR = new PagosRepositorio();
            var P = PR.ObtenerXId(id);
            return View(P);
        }

        // GET: Pagos/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.filtroContrato = TempData["filtroContrato"];
            ViewBag.Id = TempData["Id"];
            if(TempData.ContainsKey("Mensaje"))
            {
                ViewBag.Mensaje = TempData["Mensaje"];

            }
            var CR = new ContratosRepositorio();
            ViewBag.Contratos= CR.ObtenerTodos();

            var P = new Pagos();
            return View(P);
        }

        // POST: Pagos/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Pagos p)
        {
            //Validaciones
             if (p.ContratoId.Id < 0)
            {
                TempData["Mensaje"]="Debes Elegir un Contrato";
                ModelState.AddModelError("ContratoId.Id", "Debes Elegir un Contrato");
                return RedirectToAction(nameof(Create));
            }
            if(p.Importe < 0){
                TempData["Mensaje"]="El campo Importe es obligatorio";
                ModelState.AddModelError("CA", "El campo Importe es obligatorio");
                return RedirectToAction(nameof(Create));
            }
            if(DateTime.Compare(p.Fecha,DateTime.MinValue)<0){
                TempData["Mensaje"]="El campo Fecha pes obligatorio";
                ModelState.AddModelError("Fecha", "El campo Fecha p es obligatorio");
                return RedirectToAction(nameof(Create));
            }
            //ViewBag
              TempData["filtroContrato"]= p.ContratoId.Id;
            try
            {
                var PR = new PagosRepositorio();
                var CR = new ContratosRepositorio();
                var id = PR.Alta(p);
                TempData["Id"] = id;
                return RedirectToAction(nameof(Index));
            }
            catch(Exception e)
            {
                TempData["Mensaje"] = e.Message;
                Console.WriteLine(e.Message);
                return RedirectToAction(nameof(Create));
            }
        }
        [Authorize]
        public ActionResult RenovarPago(int id){
            var CR = new ContratosRepositorio();
            try{
                var C = CR.ObtenerXId(id);
                TempData["filtroContrato"]= C.Id;
            }catch(Exception e){
                TempData["Mensaje"] = e.Message;
                Console.WriteLine(e.Message);
                return RedirectToAction(nameof(Index));
            }
            
            return RedirectToAction(nameof(Create));
        }
        // GET: Pagos/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
             
            ViewBag.Id = TempData["Id"];
            if(TempData.ContainsKey("Mensaje"))
            {
                ViewBag.Mensaje = TempData["Mensaje"];

            }
            var CR = new ContratosRepositorio();

            ViewBag.Contratos= CR.ObtenerTodos();

            var PR = new PagosRepositorio();
            var P = PR.ObtenerXId(id);
            return View(P);
        }

        // POST: Pagos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(int id, Pagos p)
        {
             //Validaciones
             if (p.ContratoId.Id < 0)
            {
                TempData["Mensaje"]="Debes Elegir un Contrato";
                ModelState.AddModelError("ContratoId.Id", "Debes Elegir un Contrato");
                return RedirectToAction(nameof(Create));
            }
            if(p.Importe < 0){
                TempData["Mensaje"]="El campo Importe es obligatorio";
                ModelState.AddModelError("CA", "El campo Importe es obligatorio");
                return RedirectToAction(nameof(Create));
            }
            if(DateTime.Compare(p.Fecha,DateTime.MinValue)<0){
                TempData["Mensaje"]="El campo Fecha pes obligatorio";
                ModelState.AddModelError("Fecha", "El campo Fecha p es obligatorio");
                return RedirectToAction(nameof(Create));
            }
            try
            {
                var PR = new PagosRepositorio();
                var bol =PR.Modificacion(id,p);
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

        // GET: Pagos/Delete/5
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
            var PR = new PagosRepositorio();
            var P = PR.ObtenerXId(id);  
            return View(P);
        }

        // POST: Pagos/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Delete(int id, Pagos p)
        {
            if (User.IsInRole("Empleado"))
                {
                        TempData["Mensaje"] = "No tienes permiso de realizar esta accion";
                        return RedirectToAction(nameof(Index), "Home"); 
            }
            try
            {
                var PR = new PagosRepositorio();
                var CR = new ContratosRepositorio();
                p = PR.ObtenerXId(p.Id);
                var bol = PR.Baja(p);
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
