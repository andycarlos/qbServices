using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace qbService.Models
{
    public class EmailModel
    {
        public string UserEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
