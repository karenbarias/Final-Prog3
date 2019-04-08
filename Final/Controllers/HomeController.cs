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

        public ActionResult Nominas(string anio, string mes, string optradio)
        {
            int m = DateTime.Now.Month;
            int a = DateTime.Now.Year;

            var nominas = from n in db.CalculoNomina
                          where n.Anio == a
                          orderby n.Mes descending
                          select n;
            
            if (optradio=="mes")
            {
                m = Int32.Parse(mes);                
                nominas = from n in db.CalculoNomina
                          where n.Mes == m 
                          && n.Anio == a
                          orderby n.Mes descending
                          select n ;
            }else if (optradio=="anio")
            {
                a = Int32.Parse(anio);
                nominas = from n in db.CalculoNomina
                          where n.Anio == a
                          orderby n.Anio descending
                          select n;
            }

            return View(nominas);
        }
    }
}