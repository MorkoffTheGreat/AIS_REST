using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AIS_REST.Controllers
{
    public class MyOrderController : Controller
    {
        JsonSerializerSettings sets = new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
        };

        CRUD crud = new CRUD();

        // GET: MyOrder
        /// <summary>
        ///  Get list of all orders
        /// </summary>
        /// <returns></returns>
        [System.Web.Mvc.HttpGet]
        public string Index()
        {
            using (var context = new Models.dbEntities())
            {
                List<Models.MyOrder> orders = context.MyOrder.ToList();
                return JsonConvert.SerializeObject(orders, Formatting.Indented, sets);
            }
        }

        // GET: MyOrder/Details/5
        /// <summary>
        /// Get order by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string Details(int id)
        {
            using (var context = new Models.dbEntities())
            {
                try
                {
                    var order = context.MyOrder.Where(t => t.Order_ID == id).Single();
                    return JsonConvert.SerializeObject(order, Formatting.Indented, sets);
                }
                catch
                {
                    return "No such order!";
                }
            }
        }

        /// <summary>
        /// Create new order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [System.Web.Mvc.HttpPost]
        public string Create([FromBody] Models.MyOrder order)
        {
            using (var context = new Models.dbEntities()) {
                try
                {
                    if (order.Function == "buy")
                    {
                        Models.Student stud = context.Student.Where(t => t.Student_ID == order.Student_id).FirstOrDefault();
                        Models.Stock stock = context.Stock.Where(t => t.Stock_ID == order.Stock_id).FirstOrDefault();
                        if (order.Type == "market") order.Stock_Price = stock.Price;
                        if (stud.Currency < order.Stock_Price * order.Quantity)
                        {
                            return "бомж";
                        }
                    }
                    else
                    {
                        int stocks = context.Portfolio.Where(t => t.Student_ID == order.Student_id && t.Stocks_ID == order.Stock_id).ToList().Count(); if (stocks < order.Quantity) return "бомж но по акциям";
                    }
                    crud.Create(order);
                    return ("Order added!");
                }
                catch (Exception ex)
                {
                    return ex.Message;
                } 
            }
        }

        // POST: MyOrder/Edit/5
        /// <summary>
        /// Edit an order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [System.Web.Mvc.HttpPut]
        public string Edit([FromBody] Models.MyOrder order)
        {
            using (var context = new Models.dbEntities())
            {
                try
                {
                    var lorder = context.MyOrder.Where(t => t.Order_ID == order.Order_ID).Single();
                    lorder.Function = order.Function;
                    lorder.Quantity = order.Quantity;
                    lorder.Stock = order.Stock;
                    lorder.Stock_id = order.Stock_id;
                    lorder.Stock_Price = order.Stock_Price;
                    lorder.Student = order.Student;
                    lorder.Student_id = order.Student_id;
                    lorder.Type = order.Type;
                    context.Entry(lorder).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return "Order modified";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        // POST: MyOrder/Delete/5
        /// <summary>
        /// Delete an order by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [System.Web.Mvc.HttpPost]
        public string Delete(int id)
        {
            Models.MyOrder order;
            using (var context = new Models.dbEntities()) {
                try
                {
                    order = context.MyOrder.Where(t => t.Order_ID == id).Single();
                    if (order.Type == "limit")
                    {
                        order.Student.Currency += order.Stock_Price * order.Quantity;
                        context.Entry(order.Student).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            crud.Delete(order);
            return "Order deleted!";
        }
    }
}
