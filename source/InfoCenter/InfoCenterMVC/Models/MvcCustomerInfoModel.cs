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

        [Display(Name = "Номер телефона")]
        public string PhoneNumber { get; set; } = "";

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
    }
}