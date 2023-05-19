using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkFlowEngine.Domain.EF;

namespace WorkFlowEngine.Repository.EFConfig
{
    public class WFContext : DbContext
    {
        public WFContext(DbContextOptions options) : base(options) { }
        DbSet<Employees> Employees
        {
            get;
            set;
        }
    }
}
