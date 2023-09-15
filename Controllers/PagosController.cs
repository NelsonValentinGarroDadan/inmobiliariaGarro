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
        public ActionResult Index(int filtroContrato,DateTime filtroFecha,decimal filtroImporteMayor,decimal filtroImporteMenor)
        {
            ViewBag.filtroContrato  = filtroContrato;
            ViewBag.filtroFecha  = DateTime.Compare(filtroFecha, new DateTime(0001, 01, 01,00,00,00))  != 0  ? filtroFecha.ToString("yyyy-MM-ddTHH:mm") : "";
            ViewBag.filtroImporteMayor  = filtroImporteMayor.ToString();
            ViewBag.filtroImporteMenor  = filtroImporteMenor.ToString();
            
            if(DateTime.Compare(filtroFecha,DateTime.MaxValue) >= 0 && DateTime.Compare(filtroFecha,DateTime.MinValue) <= 0)
            {
                TempData["Mensaje"] = "La fecha no es valida, se va de rango";
                return RedirectToAction(nameof(Index));
            }
            
            ViewBag.Id = TempData["Id"];
            if(TempData.ContainsKey("Mensaje"))
            {
                ViewBag.Mensaje = TempData["Mensaje"];

            }
            var CR = new ContratosRepositorio();
            ViewBag.Contratos = CR.ObtenerTodos();
            var PR = new PagosRepositorio();
            //Logica de filtrado
            var todosC = CR.ObtenerTodos();
            var todosP = PR.ObtenerTodos();
            List<Pagos> P = new List<Pagos>();
            List<bool> condiciones = new List<bool>();

            for(int i = 0; i < todosP.Count ; i++){
                bool condicionFinal = true;
                if( filtroContrato != 0 ) condiciones.Add(filtroContrato == todosC[i].TipoEstadoId.Id ); 
                //if( DateTime.Compare(filtroFecha, new DateTime(0001, 01, 01,00,00,00))  != 0) condiciones.Add(DateTime.Compare(filtroFechaInicio,todosC[i].FechaInicio) == 0 ); 
                if( filtroImporteMayor !=0 ) condiciones.Add(filtroImporteMayor < todosP[i].Importe ); 
                if( filtroImporteMenor !=0 ) condiciones.Add(filtroImporteMenor > todosP[i].Importe ); 

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
            ViewBag.filtroContrato  = p.ContratoId;
            ViewBag.filtroFecha  = DateTime.Compare(p.Fecha, new DateTime(0001, 01, 01,00,00,00))  != 0  ? p.Fecha.ToString("yyyy-MM-ddTHH:mm") : "";
            ViewBag.filtroImport  = p.Importe.ToString();
            try
            {
                var PR = new PagosRepositorio();
                var CR = new ContratosRepositorio();
                p.ContratoId = CR.ObtenerXId(p.ContratoId.Id);
                
                //Establecer importe
                if(DateTime.Compare(p.Fecha ,p.ContratoId.FechaFin) >= 0)
                {
                    p.Importe = p.ContratoId.Importe;
                }else{
                    p.Importe = p.ContratoId.Importe*2;
                }
                
                var id = PR.Alta(p);
                
                //modificar Estado del Contrato
                p.ContratoId.TipoEstadoId.Id = 106;

                CR.Modificacion(1,p.ContratoId);

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
            try
            {
                var PR = new PagosRepositorio();
                var CR = new ContratosRepositorio();
                p = PR.ObtenerXId(p.Id);
                
                

                
                //modificar Estado del Inmueble
                
                p.ContratoId.TipoEstadoId.Id = 104;

                CR.Modificacion(1,p.ContratoId);

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
