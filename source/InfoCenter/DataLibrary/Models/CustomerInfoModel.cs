using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary.Models
{
    public class CustomerInfoModel
    {
        public int CustomerId { get; set; } = 0;
        public string CustomerName { get; set; } = "";
        public string CustomerAddress { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public int CreditCount { get; set; } = 0;
        public int CreditCountActive { get; set; } = 0;
        public int DepositCount { get; set; } = 0;
        public int DepositCountActive { get; set; } = 0;
        public int MTCount { get; set; } = 0;
    }
}
