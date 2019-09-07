using InfoCenterMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataLibrary.DataAccess;
using DataLibrary.Models;

namespace InfoCenterMVC.Classes
{
    public class CustomerData
    {
        public List<MvcCustomerInfoModel> GetCustomerInfoByPhone(string phone)
        {
            //throw new NotImplementedException();
            List<MvcCustomerInfoModel> customers = new List<MvcCustomerInfoModel>();
            DataProcessor db = new DataProcessor();
            List<CustomerInfoModel> data = db.GetCustomerInfoByPhone(phone);

            foreach (CustomerInfoModel item in data)
            {
                customers.Add(new MvcCustomerInfoModel
                {
                    CustomerId = item.CustomerId,
                    CustomerName = item.CustomerName,
                    CustomerAddress = item.CustomerAddress,
                    PhoneNumber = item.PhoneNumber,
                    CreditCount = item.CreditCount,
                    CreditCountActive = item.CreditCountActive,
                    DepositCount = item.DepositCount,
                    DepositCountActive = item.DepositCountActive,
                    MTCount = item.MTCount
                });
            }

            return customers;
        }

    }
}