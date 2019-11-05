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
        public string PhoneNumber1 { get; set; } = "";
        public string PhoneNumber2 { get; set; } = "";
        public int CreditCount { get; set; } = 0;
        public int CreditCountActive { get; set; } = 0;
        public int DepositCount { get; set; } = 0;
        public int DepositCountActive { get; set; } = 0;
        public int MTCount { get; set; } = 0;
        // Evaluate to the current date at run time
        public DateTime DateCreated
        {
            get
            {
                return this.PaymentDate.HasValue
                   ? this.PaymentDate.Value
                   : DateTime.Now;
            }

            set { this.PaymentDate = value; }
        }
        public DateTime? PaymentDate { get; set; } = null;
        public decimal PaymentSum { get; set; } = 0;
        public int OverdueDay { get; set; } = 0;
        public string LoanOfficer { get; set; } = "";
        public string LoanOfficerPhone1 { get; set; } = "";
        public string LoanOfficerPhone2 { get; set; } = "";
        public string MTName { get; set; } = "";
        public decimal MTSum { get; set; } = 0;
    }
}
