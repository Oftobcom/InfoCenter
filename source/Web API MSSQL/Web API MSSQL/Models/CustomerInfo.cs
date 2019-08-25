using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiMsql.Models
{
    public class CustomerInfo
    {
        public int Id { get; set; } = 0;
        public string CustomerName { get; set; } = "";
        public string CustomerAddres { get; set; } = "";
        public string PhoneNumber2 { get; set; } = "";
        public int CreditCount { get; set; } = 0;
        public int CreditCountActive { get; set; } = 0;
        public int DepositCount { get; set; } = 0;
        public int DepositCountActive { get; set; } = 0;
    }
}