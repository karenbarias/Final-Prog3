using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Final.Models;
using System.Data;
using System.Net;

namespace Final.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Mantenimiento()
        {
            return View();
        }

        public ActionResult Procesos()
        {
            return View();
        }

        public ActionResult Informes()
        {
            return View();
        }

        private RecursosHumanosEntities db = new RecursosHumanosEntities();

        public ActionResult Nomina(CalculoNomina c)
        {
            ViewBag.Nomina = db.Empleados.Sum(s => s.Salario);
            ViewBag.Empleados = db.Empleados.Count();

            var nomina = db.Empleados.Sum(s => s.Salario);
            
            c.Anio = DateTime.Now.Year;
            c.Mes = DateTime.Now.Month;
            c.MontoTotal = nomina;

            db.CalculoNomina.Add(c);
            db.SaveChanges();
            
            return View();
        }
    }
}