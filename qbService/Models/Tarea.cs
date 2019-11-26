using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace qbService.Models
{
    public class Tarea
    {
        public int ID { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? FinDate { get; set; }
        public string Task { get; set; }
        public string Description { get; set; }
        //public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
