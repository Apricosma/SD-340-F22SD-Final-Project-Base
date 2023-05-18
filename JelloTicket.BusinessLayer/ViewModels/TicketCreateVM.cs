using JelloTicket.DataLayer.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JelloTicket.BusinessLayer.ViewModels
{
    public class TicketCreateVM
    {public Ticket ticket { get; set; }
        public Project project { get; set; }
        public List<SelectListItem> currUsers = new List<SelectListItem>();
    }
}