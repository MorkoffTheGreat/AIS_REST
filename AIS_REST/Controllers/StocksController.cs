using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AIS_REST.Controllers
{
    [System.Web.Mvc.Route("api/[controller]")]
    public class StocksController : Controller
    {
        CRUD crud = new CRUD();
        JsonSerializerSettings sets = new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
        };
        // GET: Stocks
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("api/stocks")]
        public string Index()
        {
            using (var context = new Models.dbEntities())
            {
                List<Models.Stock> stocks = context.Stock.ToList();
                return JsonConvert.SerializeObject(stocks, Formatting.Indented, sets);
            }
        }

        // GET: Stocks/Details/5
        [System.Web.Mvc.HttpGet]
        public string Details(int id)
        {
            using (var context = new Models.dbEntities())
            {
                try
                {
                    var stock = context.Stock.Where(t => t.Stock_ID == id).Single();
                    return JsonConvert.SerializeObject(stock, Formatting.Indented, sets);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        // POST: Stocks/Create
        [System.Web.Mvc.HttpPost]
        public string Create([FromBody]Models.Stock stock)
        {
                try
                {
                    crud.Create(stock);
                    Response.StatusCode = 201;
                    return ("Stock added!");
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
        }

        // POST: Stocks/Edit/5
        [System.Web.Mvc.HttpPut]
        public string Edit([FromBody]Models.Stock stock)
        {
            using (var context = new Models.dbEntities())
            {
                try
                {
                    var lstock = context.Stock.Where(t => t.Stock_ID == stock.Stock_ID).Single();
                    lstock.Name = stock.Name;
                    lstock.Count = stock.Count;
                    lstock.Price = stock.Price;
                    lstock.MyOrder = stock.MyOrder;
                    context.Entry(lstock).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    Response.StatusCode = 202;
                    return "Stock modified";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        //// GET: Stocks/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: Stocks/Delete/5
        [System.Web.Mvc.HttpPost]
        public string Delete(int id)
        {
            using (var context = new Models.dbEntities())
            {
                try
                {
                    var stock = context.Stock.Where(t => t.Stock_ID == id).Single();
                    crud.Delete(stock);
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
