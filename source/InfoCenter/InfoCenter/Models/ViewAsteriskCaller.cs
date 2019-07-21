using System;
using System.ComponentModel.DataAnnotations;

namespace InfoCenter.Models
{
    public class ViewAsteriskCaller
    {
        //Информация о звонящем клиенте
        //1.	Id
        //2.	Номер телефона
        //3.	Время вызова

        [Display(Name = "ID вызова")]
        public int Id { get; set; }

        [Display(Name = "Номер")]
        public string Caller { get; set; }

        [Display(Name = "Дата / Время")]
        [DataType(DataType.DateTime)]
        public DateTime Date_Time { get; set; }
        
    }
}
