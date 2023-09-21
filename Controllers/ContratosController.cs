using System;
using System.Collections.Generic;
using System.Data;
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
        public ActionResult Index(int filtroEstado,int filtroInquilino, int filtroInmueble)
        {
            ViewBag.Inmueble = TempData["filtroInmueble"];
            ViewBag.Inquilino = TempData["filtroInquilino"];
            ViewBag.Importe = TempData["filtroImporte"]; 
            ViewBag.filtroEstado = TempData["filtroEstado"];
            
            ViewBag.Id = TempData["Id"];
            if(TempData.ContainsKey("Mensaje"))
            {
                ViewBag.Mensaje = TempData["Mensaje"];

            }
            var CR = new ContratosRepositorio();
            var IR = new InmueblesRepositorio();
            var UR = new UsuariosRepositorio();
            ViewBag.Inmuebles = IR.ObtenerTodos();
            ViewBag.Usuarios = UR.ObtenerTodos();
            //Logica de Filtrado
            var todosC = CR.ObtenerTodos();
            List<Contratos> C = new List<Contratos>();
            List<bool> condiciones;
            bool condicionFinal;
            for(int i = 0; i < todosC.Count ; i++){
                condicionFinal = true;
                condiciones = new List<bool>();
                if( filtroEstado != 0 ){
                    switch (filtroEstado)
                    {
                        case 1:
                                condiciones.Add(todosC[i].FechaFin >= DateTime.Now);
                                break;
                        case 2:
                                condiciones.Add(todosC[i].FechaFin < DateTime.Now);
                                break;
                    }
                }
                if( filtroInquilino !=0 ) condiciones.Add(filtroInquilino == todosC[i].InquilinoId.Id ); 
                if( filtroInmueble!= 0) condiciones.Add(filtroInmueble == todosC[i].InmuebleId.Id ); 

                for(int j = 0; j<condiciones.Count;j++) condicionFinal= condicionFinal && condiciones[j];

                if(condicionFinal) C.Add(todosC[i]);
            }
            TempData["filtroEstado"] = filtroEstado;
            TempData["filtroInmueble"] =  filtroInmueble;
            TempData["filtroImporte"] =filtroInquilino; 
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
            ViewBag.Inmueble = TempData["Inmueble"];
            ViewBag.Inquilino = TempData["Inquilino"];
            ViewBag.Importe = TempData["Importe "]; 
            ViewBag.FechaInicio =  TempData["FechaInicio"];
            ViewBag.FechaFin = TempData["FechaFin"];
            
            ViewBag.Id = TempData["Id"];
            if(TempData.ContainsKey("Mensaje"))
            {
                ViewBag.Mensaje = TempData["Mensaje"];

            }
            var UR = new UsuariosRepositorio();
            var IR = new InmueblesRepositorio();
            var CR = new ContratosRepositorio();
            ViewBag.Usuarios= UR.ObtenerTodos();
            var todosI = IR.ObtenerTodos();
            var todosC = CR.ObtenerTodos();
            List<Inmuebles> x = new List<Inmuebles>();
            for(int i = 0 ; i<todosI.Count;i++){
                   if(todosI[i].TipoEstadoId.Id != 102){
                        x.Add(todosI[i]);
                    } 
                
            }
            ViewBag.Inmuebles = x;
            var C = new Contratos();
            return View(C);
        }

        // POST: Contratos/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Contratos c)
        {
            //Validaciones
            if(c.InquilinoId.Id==0 && String.IsNullOrEmpty(c.InquilinoId.UsuarioId.DNI) && String.IsNullOrEmpty(c.InquilinoId.UsuarioId.Nombre) && String.IsNullOrEmpty(c.InquilinoId.UsuarioId.Apellido) && c.InquilinoId.UsuarioId.Telefono == 0 && String.IsNullOrEmpty(c.InquilinoId.UsuarioId.Mail))
            {
                TempData["Mensaje"]="Debes Elegir un Usuario o crear uno";
                return RedirectToAction(nameof(Create)); 
            }
            if(c.InquilinoId.Id != 0 && (!String.IsNullOrEmpty(c.InquilinoId.UsuarioId.DNI)|| !String.IsNullOrEmpty(c.InquilinoId.UsuarioId.Nombre) || !String.IsNullOrEmpty(c.InquilinoId.UsuarioId.Apellido) || c.InquilinoId.UsuarioId.Telefono != 0 || !String.IsNullOrEmpty(c.InquilinoId.UsuarioId.Mail)))
            {
               TempData["Mensaje"]="Debes Elegir un Usuario o crear uno, no puede hacer ambos";
                return RedirectToAction(nameof(Create)); 
            }
            if(c.InquilinoId.Id == 0 )
            {
                if (String.IsNullOrEmpty(c.InquilinoId.UsuarioId.DNI))
                {
                    TempData["Mensaje"]="El campo de DNI es obligatorio";
                    ModelState.AddModelError("InquilinoId.UsuarioId.DNI", "El campo DNI es obligatorio.");
                    return RedirectToAction(nameof(Create));
                }else if(!Expresiones.ValidarDNI(c.InquilinoId.UsuarioId.DNI.ToString())){
                    TempData["Mensaje"]="El DNI no es valido, se permiten solo 7 o 8 caracteres";
                    ModelState.AddModelError("InquilinoId.UsuarioId.DNI", "El DNI no es valido, se permiten solo 7 o 8 caracteres.");
                    return RedirectToAction(nameof(Create));
                }
                if (string.IsNullOrEmpty(c.InquilinoId.UsuarioId.Nombre))
                {
                    TempData["Mensaje"]="El campo de Nombre es obligatorio";
                    ModelState.AddModelError("InquilinoId.UsuarioId.Nombre", "El campo Nombre es obligatorio.");
                    return RedirectToAction(nameof(Create));
                }else if(!Expresiones.ValidarNombre(c.InquilinoId.UsuarioId.Nombre)){
                    TempData["Mensaje"]="El Nombre es valido";
                    ModelState.AddModelError("InquilinoId.UsuarioId.Nombre", "El Nombre es valido");
                    return RedirectToAction(nameof(Create));
                }
                if (string.IsNullOrEmpty(c.InquilinoId.UsuarioId.Apellido))
                {
                    TempData["Mensaje"]="El campo de Apellido es obligatorio";
                    ModelState.AddModelError("InquilinoId.UsuarioId.Apellido", "El campo Apellido es obligatorio.");
                    return RedirectToAction(nameof(Create));
                } else if(!Expresiones.ValidarApellido(c.InquilinoId.UsuarioId.Apellido)){
                    TempData["Mensaje"]="El Apellido no es valido";
                    ModelState.AddModelError("InquilinoId.UsuarioId.Apellido", "El Apellido no es valido.");
                    return RedirectToAction(nameof(Create));
                }
                if (c.InquilinoId.UsuarioId.Telefono < 0)
                {
                    TempData["Mensaje"]="El campo de Telefono es obligatorio";
                    ModelState.AddModelError("InquilinoId.UsuarioId.Telefono", "El campo Telefono es obligatorio.");
                    return RedirectToAction(nameof(Create));
                }else if(!Expresiones.ValidarTelefono(c.InquilinoId.UsuarioId.Telefono.ToString())){
                    TempData["Mensaje"]="El Telefono no es valido";
                    ModelState.AddModelError("InquilinoId.UsuarioId.Telefono", "El Telefono no es valido.");
                    return RedirectToAction(nameof(Create));
                }
                if (string.IsNullOrEmpty(c.InquilinoId.UsuarioId.Mail))
                {
                    TempData["Mensaje"]="El campo de Mail es obligatorio";
                    ModelState.AddModelError("InquilinoId.UsuarioId.Mail", "El campo Mail es obligatorio.");
                    return RedirectToAction(nameof(Create));
                }else if(!Expresiones.ValidarMail(c.InquilinoId.UsuarioId.Mail)){
                    TempData["Mensaje"]="El Mail es invalido, deberia estar en formato 'example@example.com'";
                    ModelState.AddModelError("InquilinoId.UsuarioId.Mail", "El Mail es invalido");
                    return RedirectToAction(nameof(Create));
                }
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
                ModelState.AddModelError("FechaInicio", "El campo Fecha Inicio es obligatorio");
                return RedirectToAction(nameof(Create));
            }else if (DateTime.Compare(c.FechaInicio,c.FechaFin)>0){
                TempData["Mensaje"]="La Fecha de Inicio debe ser anterior a la Fecha de Fin";
                ModelState.AddModelError("FechaInicio", "La Fecha de Inicio debe ser anterior a la Fecha de Fin");
                return RedirectToAction(nameof(Create));
            }
            if(DateTime.Compare(c.FechaFin,DateTime.MinValue)<0){
                TempData["Mensaje"]="El campo Fecha Fin es obligatorio";
                ModelState.AddModelError("FechaFin", "El campo Fecha Fin es obligatorio");
                return RedirectToAction(nameof(Create));
            }else if (DateTime.Compare(c.FechaFin,c.FechaInicio)<0){
                TempData["Mensaje"]="La Fecha de Fin debe ser posterior a la Fecha de Inicio";
                ModelState.AddModelError("FechaFin", "La Fecha de Fin debe ser posterior a la Fecha de Inicio");
                return RedirectToAction(nameof(Create));
            }
            try
            {
                
                var CR = new ContratosRepositorio();
                var todosC = CR.ObtenerTodos();
                var IR = new InmueblesRepositorio();
                var INR = new InquilinosRepositorio();
                var UR = new UsuariosRepositorio();
                c.InmuebleId = IR.ObtenerXId(c.InmuebleId.Id);
                for(int i = 0; i< todosC.Count;i++){
                    if(c.InmuebleId.Id==todosC[i].InmuebleId.Id &&  c.FechaInicio >= todosC[i].FechaInicio &&  c.FechaInicio <= todosC[i].FechaFin &&  c.FechaFin >= todosC[i].FechaInicio &&  c.FechaFin <= todosC[i].FechaFin){
                        TempData["Mensaje"]="El inmueble esta Ocupado en esas fechas";
                        return RedirectToAction(nameof(Create));
                    }
                }
                c.Id=0;
                c.InquilinoId.UsuarioId.Id = c.InquilinoId.Id == null? 0: c.InquilinoId.Id;
                c.InquilinoId.Id = c.InquilinoId.Id == null? 0: c.InquilinoId.Id;
                if(INR.Existe(c.InquilinoId)){
                    CR.Alta(c);
                    TempData["Mensaje"]="Se dio de alta el contrato y se asocio al inquilino con id:"+c.InquilinoId.Id;
                }else if(UR.Existe(c.InquilinoId.UsuarioId)){
                    INR.Alta(c.InquilinoId);
                    c.InquilinoId.Id = c.InquilinoId.UsuarioId.Id;
                    CR.Alta(c);
                    TempData["Mensaje"]="Se dio de alta el contrato y se creo al inquilino con id:"+c.InquilinoId.Id;
                }else{
                    UR.Alta(c.InquilinoId.UsuarioId);
                    c.InquilinoId.Id = c.InquilinoId.UsuarioId.Id;
                    INR.Alta(c.InquilinoId);
                    CR.Alta(c);
                    TempData["Mensaje"]="Se dio de alta el contrato , se creo al inquilino y un usuario con id:"+c.InquilinoId.Id;
                }

                TempData["Id"] = c.Id;
                //VIewBag
            TempData["Inmueble"]=  c.InmuebleId.Id;
            TempData["Inquilino"]= c.InquilinoId.Id;
            TempData["Importe "]= c.Importe.ToString();
            TempData["FechaInicio"] = DateTime.Compare(c.FechaInicio, new DateTime(0001, 01, 01,00,00,00))  != 0  ? c.FechaInicio.ToString("yyyy-MM-ddTHH:mm") : "";
            TempData["FechaFin"]= ViewBag.FechaFin = DateTime.Compare(c.FechaFin, new DateTime(0001, 01, 01,00,00,00)) != 0 ? c.FechaFin.ToString("yyyy-MM-ddTHH:mm") : " ";
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
            var CR = new ContratosRepositorio();
            var C = CR.ObtenerXId(id);
             TempData["Inmueble"]=  C.InmuebleId.Id;
            TempData["Inquilino"]= C.InquilinoId.Id;
            TempData["Importe "]= C.Importe.ToString();
            TempData["FechaInicio"] = DateTime.Compare(C.FechaInicio, new DateTime(0001, 01, 01,00,00,00))  != 0  ? C.FechaInicio.ToString("yyyy-MM-ddTHH:mm") : "";
            TempData["FechaFin"]= ViewBag.FechaFin = DateTime.Compare(C.FechaFin, new DateTime(0001, 01, 01,00,00,00)) != 0 ? C.FechaFin.ToString("yyyy-MM-ddTHH:mm") : " ";
            
            ViewBag.Inmueble = TempData["Inmueble"];
            ViewBag.Inquilino = TempData["Inquilino"];
            ViewBag.Importe = TempData["Importe "]; 
            ViewBag.FechaInicio =  TempData["FechaInicio"];
            ViewBag.FechaFin = TempData["FechaFin"];
            ViewBag.Id = TempData["Id"];
            if(TempData.ContainsKey("Mensaje"))
            {
                ViewBag.Mensaje = TempData["Mensaje"];

            }
            var UR = new UsuariosRepositorio();
            var IR = new InmueblesRepositorio();
            ViewBag.Inmuebles = IR.ObtenerTodos();
            ViewBag.Usuarios= UR.ObtenerTodos();

            
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
        [Authorize]
        public ActionResult RenovarContrato(int id){
            var CR = new ContratosRepositorio();
            try{
                var C = CR.ObtenerXId(id);
                TempData["Inmueble"] = C.InmuebleId.Id;
                TempData["Inquilino"] = C.InquilinoId.Id;
                TempData["FechaInicio"] = C.FechaFin.ToString("yyyy-MM-ddTHH:mm");
            }catch(Exception e){
                TempData["Mensaje"] = e.Message;
                Console.WriteLine(e.Message);
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Create));
        }
        [Authorize]
        public ActionResult FinalizarContrato(int id){
            var CR = new ContratosRepositorio();
            try{
                var C = CR.ObtenerXId(id);
                string Mensaje ="Se finalizo con exito el contrato. El monto total es de $"+C.Importe;
                DateTime puntoMedio = C.FechaInicio + TimeSpan.FromTicks((C.FechaFin - C.FechaInicio).Ticks / 2);
                if(puntoMedio > DateTime.Now ){
                    C.Importe = C.Importe *3;
                    Mensaje="Se finalizo con exito el contrato, pero se debe abonar una multa. El monto total es de $"+C.Importe;
                }
                C.FechaFin = DateTime.Now;
                var bol = CR.FinalizarContrato(C);
                if(bol){
                    TempData["Mensaje"] = Mensaje;
                }else{
                    TempData["Mensaje"] = "No se finalizo con exito el contrato";
                }
            }catch(Exception e){
                TempData["Mensaje"] = e.Message;
                Console.WriteLine(e.Message);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Contratos/Delete/5
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
            if (User.IsInRole("Empleado"))
                {
                        TempData["Mensaje"] = "No tienes permiso de realizar esta accion";
                        return RedirectToAction(nameof(Index), "Home"); 
            }
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
                TempData["Mensaje"] = "No puedes borrar este Contrato, porque esta asociado a un Pago";
                Console.WriteLine(e.Message);
                return RedirectToAction(nameof(Delete));
            }
        }
    }
}