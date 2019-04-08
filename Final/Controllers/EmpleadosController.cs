using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Final.Models;

namespace Final.Controllers
{
    public class EmpleadosController : Controller
    {
        private RecursosHumanosEntities db = new RecursosHumanosEntities();

        // GET: Empleados
        public ActionResult Index(string optradio, string nombre, string dep)
        {
            var empleados = db.Empleados.Where(e => e.Estatus == "Activo").Include(e => e.Cargos).Include(e => e.Departamentos);
            switch (optradio)
            {
                case "nombre": empleados = empleados.Where(e => e.Nombre == nombre || e.Apellido == nombre);
                    break;
                case "dep": empleados = empleados.Where(e => e.Departamentos.CodigoDepartamento == dep||
                e.Departamentos.Nombre == dep);
                    break;
                default: empleados = empleados;
                    break;                    
            }
            return View(empleados.ToList());
        }

        // GET: Empleados/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empleados empleados = db.Empleados.Find(id);
            if (empleados == null)
            {
                return HttpNotFound();
            }
            return View(empleados);
        }

        // GET: Empleados/Create
        public ActionResult Create()
        {
            ViewBag.Cargo = new SelectList(db.Cargos, "ID", "Cargo");
            ViewBag.Departamento = new SelectList(db.Departamentos, "ID", "CodigoDepartamento");
            return View();
        }

        // POST: Empleados/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,CodigoEmpleado,Nombre,Apellido,Telefono,Departamento,Cargo,FechaIngreso,Salario,Estatus")] Empleados empleados)
        {
            if (ModelState.IsValid)
            {
                if (empleados.FechaIngreso.ToString().Equals(""))
                    empleados.FechaIngreso = DateTime.Now;
                var codigo = empleados.FechaIngreso.Value.Year * 10000 + empleados.ID;
                empleados.CodigoEmpleado = codigo.ToString();
                empleados.Estatus = "Activo";
                db.Empleados.Add(empleados);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Cargo = new SelectList(db.Cargos, "ID", "Cargo", empleados.Cargo);
            ViewBag.Departamento = new SelectList(db.Departamentos, "ID", "CodigoDepartamento", empleados.Departamento);
            return View(empleados);
        }

        // GET: Empleados/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empleados empleados = db.Empleados.Find(id);
            if (empleados == null)
            {
                return HttpNotFound();
            }
            ViewBag.Cargo = new SelectList(db.Cargos, "ID", "Cargo", empleados.Cargo);
            ViewBag.Departamento = new SelectList(db.Departamentos, "ID", "CodigoDepartamento", empleados.Departamento);
            return View(empleados);
        }

        // POST: Empleados/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CodigoEmpleado,Nombre,Apellido,Telefono,Departamento,Cargo,FechaIngreso,Salario,Estatus")] Empleados empleados)
        {
            if (ModelState.IsValid)
            {
                db.Entry(empleados).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Cargo = new SelectList(db.Cargos, "ID", "Cargo", empleados.Cargo);
            ViewBag.Departamento = new SelectList(db.Departamentos, "ID", "CodigoDepartamento", empleados.Departamento);
            return View(empleados);
        }

        // GET: Empleados/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empleados empleados = db.Empleados.Find(id);
            if (empleados == null)
            {
                return HttpNotFound();
            }
            return View(empleados);
        }

        // POST: Empleados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Empleados empleados = db.Empleados.Find(id);
            db.Empleados.Remove(empleados);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Salida(string empleado, string optradio, string motivo, string fecha)
        {
            if (!String.IsNullOrEmpty(empleado))
            {
                string emp = empleado;
                try
                {
                    var query = (from e in db.Empleados
                                 where e.CodigoEmpleado == emp
                                 select e).First();

                    if (query.Estatus.Equals("Inactivo"))
                    {
                        ViewBag.Estatus = query.Nombre + " " + query.Apellido + " ya ha sido inactivado previamente";                        
                    }
                    else
                    {
                        try
                        {
                            query.Estatus = "Inactivo";
                            string s = "";
                            switch (optradio)
                            {
                                case "d1":
                                    s = "Despido";
                                    break;
                                case "d2":
                                    s = "Desahucio";
                                    break;
                                case "d3":
                                    s = "Renuncia";
                                    break;
                            }
                            SalidaEmpleados salida = new SalidaEmpleados
                            {
                                Empleado = query.ID,
                                Motivo = motivo,
                                FechaSalida = Convert.ToDateTime(fecha),
                                TipoSalida = s
                            };
                            db.SalidaEmpleados.Add(salida);
                            db.SaveChanges();
                            ViewBag.Estatus = query.Nombre + " " + query.Apellido + " inactivado con éxito";
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Estatus = query.Nombre + " " + query.Apellido + " no pudo ser inactivado";
                        }
                        
                    }
                }                
                catch(Exception ex)
                {
                    ViewBag.Estatus = emp + " no encontrado";
                }               

            }
            
            return View();
        }

        public ActionResult Inactivos()
        {
            var empleados = db.Empleados.Where(e => e.Estatus == "Inactivo").Include(e => e.Cargos).Include(e => e.Departamentos);
            return View(empleados.ToList());
        }

        public ActionResult Entradas(string optradio, string emes, string omes, string anio)
        {
            int mes = DateTime.Now.Month;
            int ano = DateTime.Now.Year;            

            if (optradio == "este")
            {
                mes = Int32.Parse(emes);

            }else if(optradio == "otro")
            {
                mes = Int32.Parse(omes);
                ano = Int32.Parse(anio);
            }

            var empleados = db.Empleados.Where(e => e.FechaIngreso.Value.Month == mes
                    && e.FechaIngreso.Value.Year == ano);

            return View(empleados);
        }

        public ActionResult Salidas(string optradio, string emes, string omes, string anio)
        {
            int mes = DateTime.Now.Month;
            int ano = DateTime.Now.Year;

            if (optradio == "este")
            {
                mes = Int32.Parse(emes);
            }
            else if (optradio == "otro")
            {
                mes = Int32.Parse(omes);
                ano = Int32.Parse(anio);
            }

            var empleados = db.SalidaEmpleados.Where(e => e.FechaSalida.Value.Month == mes
                    && e.FechaSalida.Value.Year == ano);

            return View(empleados);
        }
    }
}
