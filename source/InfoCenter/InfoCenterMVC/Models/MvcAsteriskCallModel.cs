using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InfoCenterMVC.Models
{
    public class MvcAsteriskCallModel
    {
        //Информация о звонящем клиенте
        //1.	Id
        //2.	Номер телефона
        //3.	Время вызова

        [Display(Name = "ID вызова")]
        public int Id { get; set; }

        [Display(Name = "Номер А")]
        public string Caller { get; set; }

        [Display(Name = "Номер Б")]
        public string Callee { get; set; }

        [Display(Name = "Дата и время вызова")]
        [DataType(DataType.DateTime)]
        public DateTime Date_Time { get; set; }
    }
}