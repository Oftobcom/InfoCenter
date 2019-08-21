using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataAccess;

namespace CallerService.Controllers
{
    public class CallerController : ApiController
    {
        public IEnumerable<ViewAsteriskCaller> Get()
        {
            using (InfoCenterDBEntities callers = new InfoCenterDBEntities())
            {
                return callers.ViewAsteriskCaller.ToList();
            }

        }

        public IEnumerable<ViewAsteriskCaller> Get(string phone)
        {
            using (InfoCenterDBEntities callers = new InfoCenterDBEntities())
            {
                return callers.ViewAsteriskCaller.Where(c => c.Caller.Contains(phone)).ToList();
            }
        }

    }
}