using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;

namespace AIS_REST
{
    public class Updater
    {
        Timer timer;
        CRUD crud;
        public void Start()
        {
            TimerCallback callbackPrices = new TimerCallback(UpdatePrices);
            timer = new Timer(callbackPrices, null, 0, 1000);
        }
        private void UpdatePrices(object obj)
        {
            using (var context = new Models.dbEntities())
            {
                foreach (Models.Stock stock in context.Stock.ToList())
                {
                    Random rng = new Random(DateTime.Now.Millisecond);
                    stock.Price += (rng.NextDouble() * stock.Price * 0.01f) * (rng.Next(-1, 2));
                    context.Entry(stock).State = System.Data.Entity.EntityState.Modified;
                }
                context.SaveChanges();
            }
            UpdateOrders();
        }

        private void UpdateOrders()
        {
            CRUD crud = new CRUD();
            using (var context = new Models.dbEntities())
            {
                foreach (Models.MyOrder order in context.MyOrder.ToList())
                {
                    if (order.Type == "market" && !order.Function.Contains('*'))
                    {
                        order.Stock_Price = order.Stock.Price;
                    }
                    switch (order.Function)
                        {
                            case "buy":
                            if (order.Stock.Price <= order.Stock_Price || order.Type == "market")
                            {
                                Models.Portfolio port;
                                for (int i = 0; i < order.Quantity; i++)
                                {
                                    port = new Models.Portfolio { Stocks_ID = order.Stock_id, Student_ID = order.Student_id };
                                    context.Portfolio.Add(port);
                                }
                                order.Stock.Count -= order.Quantity;
                                order.Function = "buy*";
                                order.Student.Currency -= order.Stock_Price * order.Quantity;
                                context.Entry(order).State = System.Data.Entity.EntityState.Modified;
                                    context.Entry(order.Stock).State = System.Data.Entity.EntityState.Modified;
                                    context.SaveChanges();
                                context.Entry(order.Student).State = System.Data.Entity.EntityState.Modified;
                            }
                                break;
                            case "sell":
                                if (order.Stock.Price >= order.Stock_Price || order.Type == "market")
                                {
                                    var ports = context.Portfolio.Where(t => t.Stocks_ID == order.Stock_id && t.Student_ID == order.Student_id).ToList();
                                    order.Stock.Count += order.Quantity;
                                    order.Function = "sell*";
                                    order.Student.Currency += order.Stock_Price * order.Quantity;
                                    foreach (var port in ports)
                                    {
                                        context.Entry(port).State = System.Data.Entity.EntityState.Deleted;
                                    }
                                    context.Entry(order.Stock).State = System.Data.Entity.EntityState.Modified;
                                    context.Entry(order.Student).State = System.Data.Entity.EntityState.Modified;
                                    context.SaveChanges();
                                }
                                break;
                        }
                    }
                context.SaveChanges();
            }
        }
    }
}