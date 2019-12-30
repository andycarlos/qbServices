using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;

namespace qbService.Models
{
    public class SaleOrderConfigModel
    {
        public int ID { get; set; }
        public int InvocesDueDate { get; set; }
        public int DaysNextDueDate { get; set; }
        public Boolean CreditLimit { get; set; }
        [Required]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
    public class SaleOrderConfigModel1
    {
        public int ID { get; set; }
        public int InvocesDueDate { get; set; }
        public int DaysNextDueDate { get; set; }
        public Boolean CreditLimit { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
