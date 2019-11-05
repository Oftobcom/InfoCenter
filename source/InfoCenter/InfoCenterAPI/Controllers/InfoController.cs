using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataLibrary.DataAccess;
using DataLibrary.Models;

namespace InfoCenterAPI.Controllers
{
    public class InfoController : ApiController
    {
        [Route("info/customerbyphone/{phone}")]
        public List<CustomerInfoModel> GetCustomerByPhone(string phone)
        {
            List<CustomerInfoModel> customers = new List<CustomerInfoModel>();
            DataProcessor db = new DataProcessor();
            customers = db.GetCustomerInfoByPhone(phone);
            return customers;

            //return new DataProcessor().GetCustomerInfoByPhone(phone);
        }

        [Route("info/asteriskcallers")]
        public List<AsteriskCallModel> GetAsteriskCallers()
        {
            List<AsteriskCallModel> callers = new List<AsteriskCallModel>();
            DataProcessor db = new DataProcessor();
            callers = db.GetAsteriskCallsInfo();
            return callers;

            //return new DataProcessor().GetCustomerInfoByPhone(phone);
        }

    }
}
