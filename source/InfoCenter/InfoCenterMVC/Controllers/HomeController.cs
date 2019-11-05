using InfoCenterMVC.Classes;
using InfoCenterMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InfoCenterMVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("CustomerSearch");
            //return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Questions()
        {
            ViewBag.Message = "Частозадаваемые вопросы и ответы тут.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult AsteriskCallers()
        {
            List<MvcAsteriskCallModel> calls = new List<MvcAsteriskCallModel>();
            AsteriskData asteriskData = new AsteriskData();
            calls = asteriskData.GetAsteriskCallsInfo();
            //AsteriskCallsViewModel asteriskInfo = new AsteriskCallsViewModel();
            //asteriskInfo.Calls = calls;
            //return View(asteriskInfo);
            return View(calls);
        }

        public ActionResult CustomerSearch()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CustomerSearch(CustomerSearchViewModel model)
        {
            if (ModelState.IsValid)
            {
                List<MvcCustomerInfoModel> customers = new List<MvcCustomerInfoModel>();
                CustomerData customersData = new CustomerData();
                customers = customersData.GetCustomerInfoByPhone(model.Phone);
                CustomerSearchViewModel customerSearch = new CustomerSearchViewModel();
                customerSearch.Customers = customers;
                customerSearch.Phone = model.Phone;
                return View(customerSearch);

                //ViewBag.Message = "Your contact page.";
            }

            return View();
        }
    }
}   