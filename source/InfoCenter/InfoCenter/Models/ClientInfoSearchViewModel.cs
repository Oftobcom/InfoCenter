using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace InfoCenter.Models
{
    public class ClientInfoSearchViewModel
    {
        public List<ClientInfo> ClientInfo { get; set; }
        public string SearchPhone { get; set; }
    }
}
