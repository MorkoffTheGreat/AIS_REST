using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AIS_REST.Controllers
{
    public class StudentsController : Controller
    {
        CRUD crud = new CRUD();

        JsonSerializerSettings sets = new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
        };
        // GET: Students
        public string Index()
        {
            using (var context = new Models.dbEntities())
            {
                List<Models.Student> studs = context.Student.ToList();
                return JsonConvert.SerializeObject(studs, Formatting.Indented, sets);
            }
        }

        // GET: Students/Details/5
        public string Details(int id)
        {
            using (var context = new Models.dbEntities())
            {
                try
                {
                    var stud = context.Student.Where(t => t.Student_ID == id).Single();
                    JsonSerializerSettings sets = new JsonSerializerSettings
                    {
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    };
                    return JsonConvert.SerializeObject(stud, Formatting.Indented, sets);
                }
                catch
                {
                    return "No such student!";
                }
            }
        }

        [System.Web.Mvc.HttpGet]
        public string GetStocks(int id)
        {
            using (var context = new Models.dbEntities())
            {
                try
                {
                    var ports = context.Portfolio.Where(t => t.Student_ID == id).ToList();
                    JsonSerializerSettings sets = new JsonSerializerSettings
                    {
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    };
                    var stocksid = new List<int>();
                    foreach (var port in ports)
                    {
                        stocksid.Add(port.Stocks_ID);
                    }
                    var stocks = new List<Models.Stock>();
                    foreach (var stock in stocksid)
                    {
                        stocks.Add(context.Stock.Where(t => t.Stock_ID == stock).Single());
                    }

                    return JsonConvert.SerializeObject(stocks, Formatting.Indented, sets);
                }
                catch(Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        [System.Web.Mvc.HttpPost]
        public string Auth([FromBody]Models.Student auth)
        {
            using (var context = new Models.dbEntities())
            {
                try
                {
                    var stud = context.Student.Where(t => t.Passport == auth.Passport).Single();
                    if (stud.Fam == auth.Fam && stud.Im == auth.Im && stud.Otch == auth.Otch)
                    {
                        return stud.Student_ID.ToString();
                    }
                    else
                    {
                        return "Wrong auth data!";
                    }
                }
                catch
                {
                    return "No student with this passport!";
                }
            }
        }

        // POST: Students/Create
        [System.Web.Mvc.HttpPost]
        public string Create([FromBody] Models.Student stud)
        {
            try
            {
                crud.Create(stud);
                return ("Student added!");
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // POST: Students/Edit/5
        [System.Web.Mvc.HttpPut]
        public string Edit([FromBody] Models.Student stud)
        {
            using (var context = new Models.dbEntities())
            {
                try
                {
                    var lstud = context.Student.Where(t => t.Student_ID == stud.Student_ID).Single();
                    lstud.Fam = stud.Fam;
                    lstud.Im = stud.Im;
                    lstud.Otch = stud.Otch;
                    lstud.group_id = stud.group_id;
                    lstud.MyOrder = stud.MyOrder;
                    lstud.Passport = stud.Passport;
                    lstud.Portfolio = stud.Portfolio;
                    lstud.Currency = stud.Currency;
                    context.Entry(lstud).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return "Student modified";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        // POST: Students/Delete/5
        [System.Web.Mvc.HttpPost]
        public string Delete(int id)
        {
            using (var context = new Models.dbEntities())
            {
                try
                {
                    var stud = context.Student.Where(t => t.Student_ID == id).Single();
                    crud.Delete(stud);
                    return "Stock deleted!";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
    }
}
