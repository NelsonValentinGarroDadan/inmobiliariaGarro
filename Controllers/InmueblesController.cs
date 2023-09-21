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
                ViewBag.filtroFechaInicio = "";
                ViewBag.filtroFechaFin = "";
                return RedirectToAction(nameof(Index));
             }
            var TUR = new TiposUsosRepositorio();
            var TER = new TiposEstadosRepositorio();
            var TIR = new TiposInmueblesRepositorio();
            var UR = new UsuariosRepositorio();
            ViewBag.Usuarios = UR.ObtenerTodos();
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
                if(DateTime.Compare(filtroFechaInicio, new DateTime(0001, 01, 01,00,00,00))  > 0){
                    for(int j=0 ; j < todosC.Count;j++){
                        if(todosC[j].InmuebleId.Id == todosI[i].Id){
                            condiciones.Add(DateTime.Compare(filtroFechaInicio,todosC[j].FechaFin) != 0 && DateTime.Compare(filtroFechaInicio,todosC[j].FechaFin) != 0);
                        }
                    }
                } 
                if(DateTime.Compare(filtroFechaFin, new DateTime(0001, 01, 01,00,00,00))  > 0){
                    for(int j=0 ; j < todosC.Count;j++){
                        if(todosC[j].InmuebleId.Id == todosI[i].Id){
                        condiciones.Add(DateTime.Compare(filtroFechaFin,todosC[j].FechaInicio) != 0 && DateTime.Compare(filtroFechaFin,todosC[j].FechaInicio) != 0);
                        }
                    }
                }
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
            ViewBag.Direccion = TempData["Direccion"] ;
            ViewBag.PropietarioId = TempData["PropietarioId "];
            ViewBag.CA =TempData["CA"];
            ViewBag.Longitud = TempData["Longitud "];
            ViewBag.Latitud = TempData["Latitud"] ;
            ViewBag.TipoUso = TempData["TipoUso"] ;
            ViewBag.TipoInmueble = TempData["TipoInmueble"];
            ViewBag.Precio =  TempData["Precio"];
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
            //Validaciones
            if(i.PropietarioId.Id==0 && String.IsNullOrEmpty(i.PropietarioId.UsuarioId.DNI) && String.IsNullOrEmpty(i.PropietarioId.UsuarioId.Nombre) && String.IsNullOrEmpty(i.PropietarioId.UsuarioId.Apellido) && i.PropietarioId.UsuarioId.Telefono == 0 && String.IsNullOrEmpty(i.PropietarioId.UsuarioId.Mail))
            {
                TempData["Mensaje"]="Debes Elegir un Usuario o crear uno";
                return RedirectToAction(nameof(Create)); 
            }
            if(i.PropietarioId.Id != 0 && (!String.IsNullOrEmpty(i.PropietarioId.UsuarioId.DNI)|| !String.IsNullOrEmpty(i.PropietarioId.UsuarioId.Nombre) || !String.IsNullOrEmpty(i.PropietarioId.UsuarioId.Apellido) || i.PropietarioId.UsuarioId.Telefono != 0 || !String.IsNullOrEmpty(i.PropietarioId.UsuarioId.Mail)))
            {
               TempData["Mensaje"]="Debes Elegir un Usuario o crear uno, no puede hacer ambos";
                return RedirectToAction(nameof(Create)); 
            }
            if(i.PropietarioId.Id == 0 )
            {
                if (String.IsNullOrEmpty(i.PropietarioId.UsuarioId.DNI))
                {
                    TempData["Mensaje"]="El campo de DNI es obligatorio";
                    ModelState.AddModelError("PropietarioId.UsuarioId.DNI", "El campo DNI es obligatorio.");
                    return RedirectToAction(nameof(Create));
                }else if(!Expresiones.ValidarDNI(i.PropietarioId.UsuarioId.DNI.ToString())){
                    TempData["Mensaje"]="El DNI no es valido, se permiten solo 7 o 8 caracteres";
                    ModelState.AddModelError("PropietarioId.UsuarioId.DNI", "El DNI no es valido, se permiten solo 7 o 8 caracteres.");
                    return RedirectToAction(nameof(Create));
                }
                if (string.IsNullOrEmpty(i.PropietarioId.UsuarioId.Nombre))
                {
                    TempData["Mensaje"]="El campo de Nombre es obligatorio";
                    ModelState.AddModelError("PropietarioId.UsuarioId.Nombre", "El campo Nombre es obligatorio.");
                    return RedirectToAction(nameof(Create));
                }else if(!Expresiones.ValidarNombre(i.PropietarioId.UsuarioId.Nombre)){
                    TempData["Mensaje"]="El Nombre es valido";
                    ModelState.AddModelError("PropietarioId.UsuarioId.Nombre", "El Nombre es valido");
                    return RedirectToAction(nameof(Create));
                }
                if (string.IsNullOrEmpty(i.PropietarioId.UsuarioId.Apellido))
                {
                    TempData["Mensaje"]="El campo de Apellido es obligatorio";
                    ModelState.AddModelError("PropietarioId.UsuarioId.Apellido", "El campo Apellido es obligatorio.");
                    return RedirectToAction(nameof(Create));
                } else if(!Expresiones.ValidarApellido(i.PropietarioId.UsuarioId.Apellido)){
                    TempData["Mensaje"]="El Apellido no es valido";
                    ModelState.AddModelError("PropietarioId.UsuarioId.Apellido", "El Apellido no es valido.");
                    return RedirectToAction(nameof(Create));
                }
                if (i.PropietarioId.UsuarioId.Telefono < 0)
                {
                    TempData["Mensaje"]="El campo de Telefono es obligatorio";
                    ModelState.AddModelError("PropietarioId.UsuarioId.Telefono", "El campo Telefono es obligatorio.");
                    return RedirectToAction(nameof(Create));
                }else if(!Expresiones.ValidarTelefono(i.PropietarioId.UsuarioId.Telefono.ToString())){
                    TempData["Mensaje"]="El Telefono no es valido";
                    ModelState.AddModelError("PropietarioId.UsuarioId.Telefono", "El Telefono no es valido.");
                    return RedirectToAction(nameof(Create));
                }
                if (string.IsNullOrEmpty(i.PropietarioId.UsuarioId.Mail))
                {
                    TempData["Mensaje"]="El campo de Mail es obligatorio";
                    ModelState.AddModelError("PropietarioId.UsuarioId.Mail", "El campo Mail es obligatorio.");
                    return RedirectToAction(nameof(Create));
                }else if(!Expresiones.ValidarMail(i.PropietarioId.UsuarioId.Mail)){
                    TempData["Mensaje"]="El Mail es invalido, deberia estar en formato 'example@example.com'";
                    ModelState.AddModelError("PropietarioId.UsuarioId.Mail", "El Mail es invalido");
                    return RedirectToAction(nameof(Create));
                }
            }
            if(String.IsNullOrEmpty(i.Direccion)){
                TempData["Mensaje"]="El campo Direccion es obligatorio";
                ModelState.AddModelError("Direccion", "El campo Direccion es obligatorio");
                return RedirectToAction(nameof(Create));
            }
            if(i.CA < 0){
                TempData["Mensaje"]="El campo Cantidad de Ambientes es obligatorio";
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
            try
            {
                var IR = new InmueblesRepositorio();
                var UR = new UsuariosRepositorio();
                var PR = new PropietariosRepositorio();
                i.Id= 0;
                i.PropietarioId.UsuarioId.Id = i.PropietarioId.Id == null? 0: i.PropietarioId.Id;
                i.PropietarioId.Id = i.PropietarioId.Id == null? 0: i.PropietarioId.Id;
                i.TipoEstadoId = new TiposEstados{Id= 101};
                Console.WriteLine(PR.Existe(i.PropietarioId));
                if(PR.Existe(i.PropietarioId)){
                    IR.Alta(i);
                    TempData["Mensaje"]="Se dio de alta el inmueble y se asocio al propietario con id:"+i.PropietarioId.Id;
                }else if(UR.Existe(i.PropietarioId.UsuarioId)){
                    PR.Alta(i.PropietarioId);
                    i.PropietarioId.Id = i.PropietarioId.UsuarioId.Id;
                    IR.Alta(i);
                    TempData["Mensaje"]="Se dio de alta el inmueble y se creo al propietario con id:"+i.PropietarioId.Id;
                }else{
                    UR.Alta(i.PropietarioId.UsuarioId);
                    i.PropietarioId.Id = i.PropietarioId.UsuarioId.Id;
                    PR.Alta(i.PropietarioId);
                    IR.Alta(i);
                    TempData["Mensaje"]="Se dio de alta el inmueble , se creo al propietario y al Usuario con id:"+i.PropietarioId.Id;
                }
                //ViewBag
                TempData["Direccion"] = i.Direccion;
                TempData["PropietarioId "]= i.PropietarioId.Id;
                TempData["CA"] = i.CA;
                TempData["Longitud "]= i.Longitud;
                TempData["Latitud"] = i.Latitud;
                TempData["TipoUso"] = i.TipoUsoId.Id;
                TempData["TipoInmueble"] = i.TipoInmuebleId.Id;
                TempData["Precio"] = i.Precio.ToString(); 
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
         [Authorize]
        public ActionResult CrearContrato(int id){
            TempData["Inmueble"] = id;
            return RedirectToAction("Create","Contratos");
        }
        // GET: Inmuebles/Delete/5
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
            if (User.IsInRole("Empleado"))
                {
                        TempData["Mensaje"] = "No tienes permiso de realizar esta accion";
                        return RedirectToAction(nameof(Index), "Home"); 
            }
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