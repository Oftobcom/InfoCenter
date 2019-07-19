using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace InfoCenter.Models
{
    public class InfoCenterContext : DbContext
    {
        public InfoCenterContext (DbContextOptions<InfoCenterContext> options)
            : base(options)
        {
        }

        public DbSet<InfoCenter.Models.ClientInfo> ClientInfo { get; set; }
    }
}
