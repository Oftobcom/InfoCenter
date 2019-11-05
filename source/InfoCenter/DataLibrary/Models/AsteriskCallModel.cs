using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary.Models
{
    public class AsteriskCallModel
    {
        //Информация о звонящем клиенте
        //	Id
        //	Номер телефона А
        //	Номер телефона Б
        //	Время вызова

        public int Id { get; set; } = 0;
        public string Caller { get; set; } = "";
        public string Callee { get; set; } = "";
        public DateTime Date_Time { get; set; }

    }
}
