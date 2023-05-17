using JelloTicket.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JelloTicket.DataLayer.Models.Ticket;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace JelloTicket.BusinessLayer.ViewModels
{
    public class TicketEditVM
    {
       public Ticket ticket { get; set; }
        public IEnumerable<SelectListItem> Users { get; set; } = new HashSet<SelectListItem>();



    }
}
