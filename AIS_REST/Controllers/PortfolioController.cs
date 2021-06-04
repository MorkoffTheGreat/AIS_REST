using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;
using Newtonsoft.Json;

namespace AIS_REST.Controllers
{
    public class PortfolioController : Controller
    {
        CRUD crud = new CRUD();
        JsonSerializerSettings sets = new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
        };
        // GET: Portfolio
        public string Index()
        {
            using (var context = new Models.dbEntities())
            {
                List<Models.Portfolio> ports = context.Portfolio.ToList();
                return JsonConvert.SerializeObject(ports, Formatting.Indented, sets);
            }
        }

        // GET: Portfolio/Details/5
        public string Details(int id)
        {
            using (var context = new Models.dbEntities())
            {
                try
                {
                    var port = context.Portfolio.Where(t => t.Student_ID == id).Single();
                    return JsonConvert.SerializeObject(port, Formatting.Indented, sets);
                }
                catch
                {
                    return "No such portfolio!";
                }
            }
        }

        // POST: Portfolio/Create
        [System.Web.Mvc.HttpPost]
        public string Create([FromBody] Models.Portfolio port)
        {
            try
            {
                crud.Create(port);
                return ("Portfolio added!");
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // POST: Portfolio/Edit/5
        [System.Web.Mvc.HttpPut]
        public string Edit([FromBody] Models.Portfolio port)
        {
            using (var context = new Models.dbEntities())
            {
                try
                {
                    var lport = context.Portfolio.Where(t => t.id == port.id).Single();
                    lport.Stocks_ID = port.Stocks_ID;
                    lport.Student_ID = port.Student_ID;
                    lport.Student = port.Student;
                    context.Entry(lport).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return "Portfolio modified";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        //// GET: Portfolio/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: Portfolio/Delete/5
        [System.Web.Mvc.HttpPost]
        public string Delete(int id)
        {
            using (var context = new Models.dbEntities())
            {
                try
                {
                    var port = context.Portfolio.Where(t => t.id == id).Single();
                    crud.Delete(port);
                    return "Portfolio entry deleted!";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
    }
}
