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
                    PhoneNumber1 = item.PhoneNumber1,
                    PhoneNumber2 = item.PhoneNumber2,
                    CreditCount = item.CreditCount,
                    CreditCountActive = item.CreditCountActive,
                    DepositCount = item.DepositCount,
                    DepositCountActive = item.DepositCountActive,
                    MTCount = item.MTCount,
                    PaymentDate = item.PaymentDate,
                    PaymentSum = item.PaymentSum,
                    OverdueDay = item.OverdueDay,
                    LoanOfficer = item.LoanOfficer,
                    LoanOfficerPhone1 = item.LoanOfficerPhone1,
                    LoanOfficerPhone2 = item.LoanOfficerPhone2,
                    MTName = item.MTName,
                    MTSum = item.MTSum
                });
            }

            return customers;
        }

    }
}