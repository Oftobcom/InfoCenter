using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InfoCenterMVC.Models
{
    public class MvcCustomerInfoModel
    {
        //Информация о клиенте
        //Фамилия Имя Отчество
        //Адрес
        //Телефон
        //Общее количество кредитов
        //Количество активных кредитов
        //Общее количество депозитов
        //Количество активных депозитов
        //Количество денежных переводов

        public int CustomerId { get; set; } = 0;

        [Display(Name = "Ф.И.О.")]
        public string CustomerName { get; set; } = "";

        [Display(Name = "Адрес")]
        public string CustomerAddress { get; set; } = "";

        [Display(Name = "Номер телефона, 1")]
        public string PhoneNumber1 { get; set; } = "";

        [Display(Name = "Номер телефона, 2")]
        public string PhoneNumber2 { get; set; } = "";

        [Display(Name = "Общее количество кредитов")]
        public int CreditCount { get; set; } = 0;

        [Display(Name = "Количество активных кредитов")]
        public int CreditCountActive { get; set; } = 0;

        [Display(Name = "Общее количество депозитов")]
        public int DepositCount { get; set; } = 0;

        [Display(Name = "Количество активных депозитов")]
        public int DepositCountActive { get; set; } = 0;

        [Display(Name = "Количество денежных переводов")]
        public int MTCount { get; set; } = 0;

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

        [Display(Name = "Дата следующего погашения")]
        public DateTime? PaymentDate { get; set; } = null;

        [Display(Name = "Сумма погашения")]
        public decimal PaymentSum { get; set; } = 0;

        [Display(Name = "Дни просрочки")]
        public int OverdueDay { get; set; } = 0;

        [Display(Name = "Кредитный эксперт")]
        public string LoanOfficer { get; set; } = "";

        [Display(Name = "Номер телефона КЭ, 1")]
        public string LoanOfficerPhone1 { get; set; } = "";

        [Display(Name = "Номер телефона КЭ, 2")]
        public string LoanOfficerPhone2 { get; set; } = "";

        [Display(Name = "Отправитель денежного перевода")]
        public string MTName { get; set; } = "";

        [Display(Name = "Сумма")]
        public decimal MTSum { get; set; } = 0;
    }
}