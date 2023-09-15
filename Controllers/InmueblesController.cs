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
    public class InmueblesController : Controller
    {
        // GET: Inmuebles
        [Authorize]
        public ActionResult Index(int filtroEstado,int filtroPropietario,int filtroTipoInmueble,int filtroTipoUso, int filtroCA,DateTime filtroFechaInicio, DateTime filtroFechaFin )
        {
            ViewBag.filtroEstado =  filtroEstado;
            ViewBag.filtroPropietario =  filtroPropietario;
            ViewBag.filtroTipoInmueble =  filtroTipoInmueble;
            ViewBag.filtroTipoUso = filtroTipoUso;
            ViewBag.filtroCA =  filtroCA;
            ViewBag.filtroFechaInicio =  DateTime.Compare(filtroFechaInicio, new DateTime(0001, 01, 01,00,00,00))  != 0  ? filtroFechaInicio.ToString("yyyy-MM-dd") : "";
            ViewBag.filtroFechaFin = DateTime.Compare(filtroFechaFin, new DateTime(0001, 01, 01,00,00,00)) != 0 ? filtroFechaFin.ToString("yyyy-MM-dd") : " ";
            ViewBag.Id = TempData["Id"];
            if(TempData.ContainsKey("Mensaje"))
            {
                ViewBag.Mensaje = TempData["Mensaje"];

            }
            if((DateTime.Compare(filtroFechaInicio, new DateTime(0001, 01, 01,00,00,00))  != 0 || DateTime.Compare(filtroFechaFin, new DateTime(0001, 01, 01,00,00,00))  != 0 )&& DateTime.Compare(filtroFechaInicio,filtroFechaFin) > 0){
                TempData["Mensaje"]="Las fechas no son validas, la fecha de inicio debe ser anterior a la fecha de fin";
                return RedirectToAction(nameof(Index));
             }
            var TUR = new TiposUsosRepositorio();
            var TER = new TiposEstadosRepositorio();
            var TIR = new TiposInmueblesRepositorio();
            var PR = new PropietariosRepositorio();
            ViewBag.Propietarios = PR.ObtenerTodos();
            ViewBag.TiposUsos= TUR.ObtenerTodos();
            ViewBag.TiposEstados= TER.ObtenerTodos();
            ViewBag.TiposInmueble= TIR.ObtenerTodos();
            //Logica de filtrado
            var IR = new InmueblesRepositorio();
            var CR = new ContratosRepositorio();
            var todosI = IR.ObtenerTodos();
            var todosC = CR.ObtenerTodos();

            List<Inmuebles> I= new List<Inmuebles>();
            List<bool> condiciones;
            bool condicionFinal;    
            for(int i = 0; i < todosI.Count ; i++)
            { 
                condiciones = new List<bool>();
                condicionFinal = true;
                if(filtroEstado != 0) condiciones.Add(filtroEstado == todosI[i].TipoEstadoId.Id);
                if(filtroPropietario !=0) condiciones.Add(filtroPropietario == todosI[i].PropietarioId.Id);
                if(filtroCA!= 0) condiciones.Add(filtroCA == todosI[i].CA);
                if(filtroTipoInmueble != 0 ) condiciones.Add(filtroTipoInmueble == todosI[i].TipoInmuebleId.Id);
                if(filtroTipoUso != 0) condiciones.Add(filtroTipoUso == todosI[i].TipoUsoId.Id);
                //if(DateTime.Compare(filtroFechaInicio, new DateTime(0001, 01, 01,00,00,00))  > 0) condiciones.Add(todosC[i].InmuebleId.Id == todosI[i].Id && DateTime.Compare(filtroFechaInicio,todosC[i].FechaFin) > 0 && DateTime.Compare(filtroFechaInicio,todosI[i].FechaFin) > 0);
                //if(DateTime.Compare(filtroFechaFin, new DateTime(0001, 01, 01,00,00,00))  > 0) condiciones.Add(todosC[i].InmuebleId.Id == todosI[i].Id && DateTime.Compare(filtroFechaFin,todosC[i].FechaInicio) < 0 && DateTime.Compare(filtroFechaFin,todosI[i].FechaInicio) < 0);
                for(int j = 0; j<condiciones.Count;j++) condicionFinal= condicionFinal && condiciones[j];
                if(condicionFinal) I.Add(todosI[i]);
            }

            
            return View(I);
        }

        // GET: Inmuebles/Details/5
        [Authorize]
        public ActionResult Details(int id)
        {
            
            var IR = new InmueblesRepositorio();
            var I = IR.ObtenerXId(id);
            return View(I);
        }

        // GET: Inmuebles/Create
        [Authorize]
        public ActionResult Create()
        {
            
            ViewBag.Id = TempData["Id"];
            if(TempData.ContainsKey("Mensaje"))
            {
                ViewBag.Mensaje = TempData["Mensaje"];

            }
            var UR = new UsuariosRepositorio();
            var TUR = new TiposUsosRepositorio();
            var TER = new TiposEstadosRepositorio();
            var TIR = new TiposInmueblesRepositorio();

            ViewBag.Usuarios= UR.ObtenerTodos();
            ViewBag.TiposUsos= TUR.ObtenerTodos();
            ViewBag.TiposEstados= TER.ObtenerTodos();
            ViewBag.TiposInmueble= TIR.ObtenerTodos();

            var I = new Inmuebles();
            return View(I);
        }

        // POST: Inmuebles/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Inmuebles i)
        {
            //ViewBag
            ViewBag.Direccion = i.Direccion;
            ViewBag.PropietarioId = i.PropietarioId.Id;
            ViewBag.CA = i.CA;
            ViewBag.Longitud = i.Longitud;
            ViewBag.Latitud = i.Latitud;
            ViewBag.TipoUso = i.TipoUsoId.Id;
            ViewBag.TipoInmueble = i.TipoInmuebleId.Id;
            ViewBag.Precio = i.Precio.ToString();
            ViewBag.FechaInicio = DateTime.Compare(i.FechaInicio, new DateTime(0001, 01, 01,00,00,00))  != 0  ? i.FechaInicio.ToString("yyyy-MM-dd") : "";
            ViewBag.FechaIFin = DateTime.Compare(i.FechaFin, new DateTime(0001, 01, 01,00,00,00)) != 0 ? i.FechaFin.ToString("yyyy-MM-dd") : " ";
            //Validaciones
            if(String.IsNullOrEmpty(i.Direccion)){
                TempData["Mensaje"]="El campo Direccion es obligatorio";
                ModelState.AddModelError("Direccion", "El campo Direccion es obligatorio");
                return RedirectToAction(nameof(Create));
            }
            if (i.PropietarioId.Id < 0)
            {
                TempData["Mensaje"]="Debes Elegir un Propietario";
                ModelState.AddModelError("PropietarioId.Id", "Debes Elegir un Propietario");
                return RedirectToAction(nameof(Create));
            }
            if(i.CA < 0){
                TempData["Mensaje"]="El campo CA es obligatorio";
                ModelState.AddModelError("CA", "El campo CA es obligatorio");
                return RedirectToAction(nameof(Create));
            }
            if (String.IsNullOrEmpty(i.Longitud))
            {
                TempData["Mensaje"]="El campo Longitud es obligatorio";
                ModelState.AddModelError("Longitud", "El campo Longitud es obligatorio");
                return RedirectToAction(nameof(Create));
            }else if(!Expresiones.ValidarLongitud(i.Longitud)){
                TempData["Mensaje"]="La Longitud es invalida";
                ModelState.AddModelError("Longitud", "La Longitud es invalida");
                return RedirectToAction(nameof(Create));
            }
            if (String.IsNullOrEmpty(i.Latitud))
            {
                TempData["Mensaje"]="El campo Latitud es obligatorio";
                ModelState.AddModelError("Latitud", "El campo Latitud es obligatorio");
                return RedirectToAction(nameof(Create));
            }else if(!Expresiones.ValidarLatitud(i.Latitud)){
                TempData["Mensaje"]="La Latitud es invalida";
                ModelState.AddModelError("Latitud", "La Latitud es invalida");
                return RedirectToAction(nameof(Create));
            }
            if(i.TipoUsoId.Id < 0){
                TempData["Mensaje"]="Debes elegir un tipo de uso";
                ModelState.AddModelError("TipoUsoId.Id", "Debes elegir un tipo de uso");
                return RedirectToAction(nameof(Create));
            }
            if(i.TipoInmuebleId.Id < 0){
                TempData["Mensaje"]="Debes elegir un tipo de inmueble";
                ModelState.AddModelError("TipoInmuebleId.Id", "Debes elegir un tipo de inmueble");
                return RedirectToAction(nameof(Create));
            }
            if(i.Precio < 0){
                TempData["Mensaje"]="El campo Precio es obligatorio";
                ModelState.AddModelError("Precio", "El campo Precio es obligatorio");
                return RedirectToAction(nameof(Create));
            }
            if(DateTime.Compare(i.FechaInicio,DateTime.MinValue)<0){
                TempData["Mensaje"]="El campo Fecha Inicio es obligatorio";
                ModelState.AddModelError("FechaInicio", "El campo Fecha Inicio es obligatorio");
                return RedirectToAction(nameof(Create));
            }else if (DateTime.Compare(i.FechaInicio,i.FechaFin)>0){
                TempData["Mensaje"]="La Fecha de Inicio debe ser anterior a la Fecha de Fin";
                ModelState.AddModelError("FechaInicio", "La Fecha de Inicio debe ser anterior a la Fecha de Fin");
                return RedirectToAction(nameof(Create));
            }
            if(DateTime.Compare(i.FechaFin,DateTime.MinValue)<0){
                TempData["Mensaje"]="El campo Fecha Fin es obligatorio";
                ModelState.AddModelError("FechaFin", "El campo Fecha Fin es obligatorio");
                return RedirectToAction(nameof(Create));
            }else if (DateTime.Compare(i.FechaFin,i.FechaInicio)<0){
                TempData["Mensaje"]="La Fecha de Fin debe ser posterior a la Fecha de Inicio";
                ModelState.AddModelError("FechaFin", "La Fecha de Fin debe ser posterior a la Fecha de Inicio");
                return RedirectToAction(nameof(Create));
            }
            try
            {
                var IR = new InmueblesRepositorio();
                var PR = new PropietariosRepositorio();
                try{
                    PR.Alta(i.PropietarioId);
                    IR.Alta(i);
                }catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    i.PropietarioId = PR.ObtenerXId(i.PropietarioId.Id);
                    IR.Alta(i);
                    TempData["Mensaje"] = "Se dio de alta con exito pero no se creo un nuevo propietario";
                }
                TempData["Id"] = i.Id;
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
        public ActionResult Deshabilitar(int id){
            var IR = new InmueblesRepositorio();
            var I = IR.ObtenerXId(id);
            IR.Deshabilitar(I);
            return RedirectToAction(nameof(Index));
        }
        // GET: Inmuebles/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            
            ViewBag.Id = TempData["Id"];
            if(TempData.ContainsKey("Mensaje"))
            {
                ViewBag.Mensaje = TempData["Mensaje"];

            }
            var UR = new UsuariosRepositorio();
            var TUR = new TiposUsosRepositorio();
            var TER = new TiposEstadosRepositorio();
            var TIR = new TiposInmueblesRepositorio();

            ViewBag.Usuarios= UR.ObtenerTodos();
            ViewBag.TiposUsos= TUR.ObtenerTodos();
            ViewBag.TiposEstados= TER.ObtenerTodos();
            ViewBag.TiposInmueble= TIR.ObtenerTodos();

            var IR = new InmueblesRepositorio();
            var I = IR.ObtenerXId(id);
            return View(I);
        }

        // POST: Inmuebles/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Inmuebles i)
        {
            //Validaciones
            if(String.IsNullOrEmpty(i.Direccion)){
                TempData["Mensaje"]="El campo Direccion es obligatorio";
                ModelState.AddModelError("Direccion", "El campo Direccion es obligatorio");
                return RedirectToAction(nameof(Create));
            }
            if (i.PropietarioId.Id < 0)
            {
                TempData["Mensaje"]="Debes Elegir un Propietario";
                ModelState.AddModelError("UsuarioId.Id", "Debes Elegir un Propietario");
                return RedirectToAction(nameof(Create));
            }
            if(i.CA < 0){
                TempData["Mensaje"]="El campo CA es obligatorio";
                ModelState.AddModelError("CA", "El campo CA es obligatorio");
                return RedirectToAction(nameof(Create));
            }
            if (String.IsNullOrEmpty(i.Longitud))
            {
                TempData["Mensaje"]="El campo Longitud es obligatorio";
                ModelState.AddModelError("Longitud", "El campo Longitud es obligatorio");
                return RedirectToAction(nameof(Create));
            }else if(!Expresiones.ValidarLongitud(i.Longitud)){
                TempData["Mensaje"]="La Longitud es invalida";
                ModelState.AddModelError("Longitud", "La Longitud es invalida");
                return RedirectToAction(nameof(Create));
            }
            if (String.IsNullOrEmpty(i.Latitud))
            {
                TempData["Mensaje"]="El campo Latitud es obligatorio";
                ModelState.AddModelError("Latitud", "El campo Latitud es obligatorio");
                return RedirectToAction(nameof(Create));
            }else if(!Expresiones.ValidarLatitud(i.Latitud)){
                TempData["Mensaje"]="La Latitud es invalida";
                ModelState.AddModelError("Latitud", "La Latitud es invalida");
                return RedirectToAction(nameof(Create));
            }
            if(i.TipoUsoId.Id < 0){
                TempData["Mensaje"]="Debes elegir un tipo de uso";
                ModelState.AddModelError("TipoUsoId.Id", "Debes elegir un tipo de uso");
                return RedirectToAction(nameof(Create));
            }
            if(i.TipoInmuebleId.Id < 0){
                TempData["Mensaje"]="Debes elegir un tipo de inmueble";
                ModelState.AddModelError("TipoInmuebleId.Id", "Debes elegir un tipo de inmueble");
                return RedirectToAction(nameof(Create));
            }
            if(i.Precio < 0){
                TempData["Mensaje"]="El campo Precio es obligatorio";
                ModelState.AddModelError("Precio", "El campo Precio es obligatorio");
                return RedirectToAction(nameof(Create));
            }
            if(DateTime.Compare(i.FechaInicio,DateTime.MinValue)<0){
                TempData["Mensaje"]="El campo Fecha Inicio es obligatorio";
                ModelState.AddModelError("Precio", "El campo Fecha Inicio es obligatorio");
                return RedirectToAction(nameof(Create));
            }else if (DateTime.Compare(i.FechaInicio,i.FechaFin)>0){
                TempData["Mensaje"]="La Fecha de Inicio debe ser anterior a la Fecha de Fin";
                ModelState.AddModelError("Precio", "La Fecha de Inicio debe ser anterior a la Fecha de Fin");
                return RedirectToAction(nameof(Create));
            }
            if(DateTime.Compare(i.FechaFin,DateTime.MinValue)<0){
                TempData["Mensaje"]="El campo Fecha Fin es obligatorio";
                ModelState.AddModelError("Precio", "El campo Fecha Fin es obligatorio");
                return RedirectToAction(nameof(Create));
            }else if (DateTime.Compare(i.FechaFin,i.FechaInicio)<0){
                TempData["Mensaje"]="La Fecha de Fin debe ser posterior a la Fecha de Inicio";
                ModelState.AddModelError("Precio", "La Fecha de Fin debe ser posterior a la Fecha de Inicio");
                return RedirectToAction(nameof(Create));
            }
            try
            {
                
                var IR = new InmueblesRepositorio();
                var bol =IR.Modificacion(id,i);
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

        // GET: Inmuebles/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
            
            ViewBag.Id = TempData["Id"];
            if(TempData.ContainsKey("Mensaje"))
            {
                ViewBag.Mensaje = TempData["Mensaje"];

            }
            var IR = new InmueblesRepositorio();
            var I =IR.ObtenerXId(id);  
            return View(I);
        }

        // POST: Inmuebles/Delete/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Inmuebles i)
        {
             try
            {
                var IR = new InmueblesRepositorio();
                var bol = IR.Baja(i);
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