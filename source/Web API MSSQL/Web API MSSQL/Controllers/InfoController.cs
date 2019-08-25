using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Web_API_MSSQL.Classes;
using WebApiMsql.Models;

namespace WebApiMsql.Controllers
{
    public class InfoController : ApiController
    {
        //List<CustomerInfo> customers = new List<CustomerInfo>();

        //public InfoController()
        //{
        //    //customers.Add(new CustomerInfo { CustomerName = "Рахматджон Хакимов", CustomerAddres = "г.Худжанд, 190 - 30 -290" });
        //    //customers.Add(new CustomerInfo { CustomerName = "Замира Салимова", CustomerAddres = "г.Худжанд, ул. Самаркандская, 190" });
        //}

        //[Route("info/customerbyphone")]
        //public List<CustomerInfo> GetCustomerByPhone()
        //{
        //    return customers;
        //}

        [Route("info/customerbyphone/{phone}")]
        public List<CustomerInfo> GetCustomerByPhone(string phone)
        {
            //List<CustomerInfo> customers = new List<CustomerInfo>();
            //DataAccess db = new DataAccess();
            //customers = db.GetCustomerInfo(phone);
            //return customers;

            return new DataAccess().GetCustomerInfo(phone);
        }
    }
}
