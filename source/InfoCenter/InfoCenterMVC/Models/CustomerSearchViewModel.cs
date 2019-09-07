using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InfoCenterMVC.Models
{
    public class CustomerSearchViewModel
    {
        private List<MvcCustomerInfoModel> _customers;

        public List<MvcCustomerInfoModel> Customers { get => _customers; set => _customers = value; }

        [Display(Name = "Номер телефона")]
        [Required(ErrorMessage = "Необходимо ввести номер телефона клиента")]
        [StringLength(20, ErrorMessage = "Максимальная длина - 20 символов")]
        public string Phone { get; set; }
    }
}