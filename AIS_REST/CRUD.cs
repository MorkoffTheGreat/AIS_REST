using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIS_REST
{
    public class CRUD
    {
        public void Create(object objCreate)
        {
            using (var context = new Models.dbEntities())
            {
                context.Set(objCreate.GetType()).Add(objCreate);
                context.SaveChanges();
            }
        }

        public void Delete(object objDelete)
        {
            using (var context = new Models.dbEntities())
            {
                context.Set(objDelete.GetType()).Attach(objDelete);
                context.Entry(objDelete).State = System.Data.Entity.EntityState.Deleted;
                context.SaveChanges();
            }
        }
    }
}