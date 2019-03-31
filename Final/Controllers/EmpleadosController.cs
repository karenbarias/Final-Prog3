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
        public ActionResult Index()
        {
            var empleados = db.Empleados.Include(e => e.Cargos).Include(e => e.Departamentos);
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
    }
}
