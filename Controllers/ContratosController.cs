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
    public class ContratosController : Controller
    {
        // GET: Contratos
        [Authorize]
        public ActionResult Index(int filtroEstado,int filtroInquilino, int filtroInmueble,DateTime filtroFechaInicio, DateTime filtroFechaFin,decimal filtroImporteMenor,decimal filtroImporteMayor)
        {
            ViewBag.filtroEstado = filtroEstado;
            ViewBag.filtroInmueble =  filtroInmueble;
            ViewBag.filtroInquilino =filtroInquilino;
            ViewBag.filtroImporteMayor =  filtroImporteMayor.ToString();
            ViewBag.filtroImporteMenor =  filtroImporteMenor.ToString();
            ViewBag.filtroFechaInicio =  DateTime.Compare(filtroFechaInicio, new DateTime(0001, 01, 01,00,00,00))  != 0  ? filtroFechaInicio.ToString("yyyy-MM-ddTHH:mm") : "";
            ViewBag.filtroFechaFin = DateTime.Compare(filtroFechaFin, new DateTime(0001, 01, 01,00,00,00)) != 0 ? filtroFechaFin.ToString("yyyy-MM-ddTHH:mm") : " ";
            
            ViewBag.Id = TempData["Id"];
            if(TempData.ContainsKey("Mensaje"))
            {
                ViewBag.Mensaje = TempData["Mensaje"];

            }
            var CR = new ContratosRepositorio();
            var TER = new TiposEstadosRepositorio();
            var IR = new InmueblesRepositorio();
            ViewBag.Inmuebles = IR.ObtenerTodos();
            ViewBag.TiposEstados= TER.ObtenerTodos();
            //Logica de Filtrado
            var todosC = CR.ObtenerTodos();
            List<Contratos> C = new List<Contratos>();
            List<bool> condiciones = new List<bool>();
            
            for(int i = 0; i < todosC.Count ; i++){
                bool condicionFinal = true;
                if( filtroEstado != 0 ) condiciones.Add(filtroEstado == todosC[i].TipoEstadoId.Id ); 
                if( filtroInquilino !=0 ) condiciones.Add(filtroInquilino == todosC[i].InquilinoId.Id ); 
                if( filtroInmueble!= 0) condiciones.Add(filtroInmueble == todosC[i].InmuebleId.Id ); 
                //if( DateTime.Compare(filtroFechaInicio, new DateTime(0001, 01, 01,00,00,00))  != 0) condiciones.Add(DateTime.Compare(filtroFechaInicio,todosC[i].FechaInicio) == 0 ); 
                //if( DateTime.Compare(filtroFechaFin, new DateTime(0001, 01, 01,00,00,00))  != 0) condiciones.Add(DateTime.Compare(filtroFechaFin,todosC[i].FechaFin) == 0); 
                if( filtroImporteMayor !=0 ) condiciones.Add(filtroImporteMayor < todosC[i].Importe ); 
                if( filtroImporteMenor !=0 ) condiciones.Add(filtroImporteMenor > todosC[i].Importe ); 

                for(int j = 0; j<condiciones.Count;j++) condicionFinal= condicionFinal && condiciones[j];

                if(condicionFinal) C.Add(todosC[i]);
            }
            
            return View(C);
        }

        // GET: Contratos/Details/5
        [Authorize]
        public ActionResult Details(int id)
        {
            
            var CR = new ContratosRepositorio();
            var C = CR.ObtenerXId(id);
            return View(C);
        }

        // GET: Contratos/Create
        [Authorize]
        public ActionResult Create()
        {
            
            ViewBag.Id = TempData["Id"];
            if(TempData.ContainsKey("Mensaje"))
            {
                ViewBag.Mensaje = TempData["Mensaje"];

            }
            var UR = new UsuariosRepositorio();
            var IR = new InmueblesRepositorio();
            ViewBag.Usuarios= UR.ObtenerTodos();
            ViewBag.Inmuebles= IR.ObtenerTodos();

            var C = new Contratos();
            return View(C);
        }

        // POST: Contratos/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Contratos c)
        {
            //VIewBag
            ViewBag.Estado = c.TipoEstadoId;
            ViewBag.Inmueble =  c.InmuebleId;
            ViewBag.Inquilino = c.InquilinoId;
            ViewBag.Importe =  c.Importe.ToString();
            ViewBag.FechaInicio =  DateTime.Compare(c.FechaInicio, new DateTime(0001, 01, 01,00,00,00))  != 0  ? c.FechaInicio.ToString("yyyy-MM-ddTHH:mm") : "";
            ViewBag.FechaFin = DateTime.Compare(c.FechaFin, new DateTime(0001, 01, 01,00,00,00)) != 0 ? c.FechaFin.ToString("yyyy-MM-ddTHH:mm") : " ";
            
            //Validaciones
            if (c.InquilinoId.Id < 0)
            {
                TempData["Mensaje"]="Debes elegir un Inquilino";
                ModelState.AddModelError("InquilinoId.Id", "Debes elegir un inquilino");
                return RedirectToAction(nameof(Create));
            }
            if(c.InmuebleId.Id < 0){
                TempData["Mensaje"]="Debes elegir un Inmueble";
                ModelState.AddModelError("InmuebleId.Id ", "Debes elegir un Inmueble");
                return RedirectToAction(nameof(Create));
            }
            if (c.Importe < 0)
            {
                TempData["Mensaje"]="El Importe es obligatorio";
                ModelState.AddModelError("Importe", "El campo Importe es obligatorio");
                return RedirectToAction(nameof(Create));
            } 
            if(DateTime.Compare(c.FechaInicio,DateTime.MinValue)<0){
                TempData["Mensaje"]="El campo Fecha Inicio es obligatorio";
                ModelState.AddModelError("Precio", "El campo Fecha Inicio es obligatorio");
                return RedirectToAction(nameof(Create));
            }else if (DateTime.Compare(c.FechaInicio,c.FechaFin)>0){
                TempData["Mensaje"]="La Fecha de Inicio debe ser anterior a la Fecha de Fin";
                ModelState.AddModelError("Precio", "La Fecha de Inicio debe ser anterior a la Fecha de Fin");
                return RedirectToAction(nameof(Create));
            }
            if(DateTime.Compare(c.FechaFin,DateTime.MinValue)<0){
                TempData["Mensaje"]="El campo Fecha Fin es obligatorio";
                ModelState.AddModelError("c.FechaInicio", "El campo Fecha Fin es obligatorio");
                return RedirectToAction(nameof(Create));
            }else if (DateTime.Compare(c.FechaFin,c.FechaInicio)<0){
                TempData["Mensaje"]="La Fecha de Fin debe ser posterior a la Fecha de Inicio";
                ModelState.AddModelError("c.FechaFin", "La Fecha de Fin debe ser posterior a la Fecha de Inicio");
                return RedirectToAction(nameof(Create));
            }
            try
            {
                
                var CR = new ContratosRepositorio();
                var todosC = CR.ObtenerTodos();
                var IR = new InmueblesRepositorio();
                var INR = new InquilinosRepositorio();
                c.InmuebleId = IR.ObtenerXId(c.InmuebleId.Id);
                //Validar fechas
                if(DateTime.Compare(c.FechaInicio,c.InmuebleId.FechaInicio)<0||DateTime.Compare(c.FechaFin,c.InmuebleId.FechaFin)>0){
                    TempData["Mensaje"]="El inmueble no esta Habilitado en esas fechas";
                    return RedirectToAction(nameof(Create));
                }
                for(int i = 0; i< todosC.Count;i++){
                    if(DateTime.Compare(c.FechaInicio,todosC[i].FechaFin) < 0 || DateTime.Compare(c.FechaFin,todosC[i].FechaInicio) > 0){
                        TempData["Mensaje"]="El inmueble esta Ocupado en esas fechas";
                        return RedirectToAction(nameof(Create));
                    }
                }
                try{
                    INR.Alta(c.InquilinoId);
                    CR.Alta(c);
                }catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    c.InquilinoId = INR.ObtenerXId(c.InquilinoId.Id);
                    CR.Alta(c);
                    TempData["Mensaje"] = "Se dio de alta con exito pero no se creo un nuevo inquilino";
                }

                TempData["Id"] = c.Id;
                return RedirectToAction(nameof(Index));
            }
            catch(Exception e)
            {
                TempData["Mensaje"] = e.Message;
                return RedirectToAction(nameof(Create));
            }
        }
        
        // GET: Contratos/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            
            ViewBag.Id = TempData["Id"];
            if(TempData.ContainsKey("Mensaje"))
            {
                ViewBag.Mensaje = TempData["Mensaje"];

            }
            var UR = new UsuariosRepositorio();
            var TER = new TiposEstadosRepositorio();

            ViewBag.Usuarios= UR.ObtenerTodos();
            ViewBag.TiposEstados= TER.ObtenerTodos();

            var CR = new ContratosRepositorio();
            var C = CR.ObtenerXId(id);
            return View(C);
        }

        // POST: Contratos/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Contratos c)
        { 
            //Validaciones
            if (c.InquilinoId.Id < 0)
            {
                TempData["Mensaje"]="Debes elegir un Inquilino";
                ModelState.AddModelError("InquilinoId.Id", "Debes elegir un inquilino");
                return RedirectToAction(nameof(Create));
            }
            if(c.InmuebleId.Id < 0){
                TempData["Mensaje"]="Debes elegir un Inmueble";
                ModelState.AddModelError("InmuebleId.Id ", "Debes elegir un Inmueble");
                return RedirectToAction(nameof(Create));
            }
            if (c.Importe < 0)
            {
                TempData["Mensaje"]="El Importe es obligatorio";
                ModelState.AddModelError("Importe", "El campo Importe es obligatorio");
                return RedirectToAction(nameof(Create));
            } 
            if(DateTime.Compare(c.FechaInicio,DateTime.MinValue)<0){
                TempData["Mensaje"]="El campo Fecha Inicio es obligatorio";
                ModelState.AddModelError("Precio", "El campo Fecha Inicio es obligatorio");
                return RedirectToAction(nameof(Create));
            }else if (DateTime.Compare(c.FechaInicio,c.FechaFin)>0){
                TempData["Mensaje"]="La Fecha de Inicio debe ser anterior a la Fecha de Fin";
                ModelState.AddModelError("Precio", "La Fecha de Inicio debe ser anterior a la Fecha de Fin");
                return RedirectToAction(nameof(Create));
            }
            if(DateTime.Compare(c.FechaFin,DateTime.MinValue)<0){
                TempData["Mensaje"]="El campo Fecha Fin es obligatorio";
                ModelState.AddModelError("Precio", "El campo Fecha Fin es obligatorio");
                return RedirectToAction(nameof(Create));
            }else if (DateTime.Compare(c.FechaFin,c.FechaInicio)<0){
                TempData["Mensaje"]="La Fecha de Fin debe ser posterior a la Fecha de Inicio";
                ModelState.AddModelError("Precio", "La Fecha de Fin debe ser posterior a la Fecha de Inicio");
                return RedirectToAction(nameof(Create));
            }
            try
            {
                
                var CR = new ContratosRepositorio();
                var bol =CR.Modificacion(id,c);
                if(bol)
                {
                  TempData["Mensaje"]="Se modifico con exito la entidad id:"+c.Id;
                return RedirectToAction(nameof(Index));  
                }else
                {
                    throw new Exception("No se pudo modificar con exito la entidad con id:"+c.Id);
                }
                
            }
            catch(Exception e)
            {
                TempData["Mensaje"] = e.Message;
                Console.WriteLine(e.Message);
                return RedirectToAction(nameof(Edit));
            }
        }

        // GET: Contratos/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
            
            ViewBag.Id = TempData["Id"];
            if(TempData.ContainsKey("Mensaje"))
            {
                ViewBag.Mensaje = TempData["Mensaje"];

            }
            var CR = new ContratosRepositorio();
            var C =CR.ObtenerXId(id);  
            return View(C);
        }

        // POST: Contratos/Delete/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Contratos c)
        {
             try
            {
                var CR = new ContratosRepositorio();
                var IR = new InmueblesRepositorio();
                c = CR.ObtenerXId(c.Id);
                
                

                
                //modificar Estado del Inmueble
                
                c.InmuebleId.TipoEstadoId.Id = 101;

                IR.Modificacion(1,c.InmuebleId);

                var bol = CR.Baja(c);
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