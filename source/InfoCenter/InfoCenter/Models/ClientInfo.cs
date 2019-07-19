using System;
using System.ComponentModel.DataAnnotations;

namespace InfoCenter.Models
{
    public class ClientInfo
    {
        //Информация о клиенте
        //1.	Фамилия
        //2.	Имя
        //3.	Отчество
        //4.	Адрес
        //5.	Телефон
        //6.	Общее количество кредитов
        //7.	Количество активных кредитов
        //8.	Общее количество депозитов
        //9.	Количество активных депозитов
        //10.	Количество денежных переводов

        public int Id { get; set; }

        [Display(Name = "Фамилия")]
        public string Surname { get; set; }

        [Display(Name = "Имя")]
        public string Firstname { get; set; }

        [Display(Name = "Отчество")]
        public string Patronymic { get; set; }

        [Display(Name = "Адрес")]
        public string Address { get; set; }

        [Display(Name = "Телефон")]
        public string Phone { get; set; }

        [Display(Name = "Общее количество кредитов")]
        public int NumberTotalCredits { get; set; }

        [Display(Name = "Количество активных кредитов")]
        public int NumberActiveCredits { get; set; }

        [Display(Name = "Общее количество депозитов")]
        public int NumberTotalDeposits { get; set; }

        [Display(Name = "Количество активных депозитов")]
        public int NumberActiveDeposits { get; set; }

        [Display(Name = "Количество денежных переводов")]
        public int NumberRemittances { get; set; }

    }
}
