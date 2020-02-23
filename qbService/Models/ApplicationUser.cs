using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace qbService.Models
{
    public class ApplicationUser: IdentityUser
    {
        public string CompanyName { get; set; }
        public DateTime CreateTimer { get; set; }
        public string TypeUser { get; set; }
        public string Name { get; set; }
        public string ListID { get; set; }

        public List<Tarea> Tareas { get; set; }
        public SaleOrderConfigModel SaleOrderConfigModel { get; set; }
    }
}
