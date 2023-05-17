using JelloTicket.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JelloTicket.BusinessLayer.ViewModels
{
    public  class TicketIndex
    {
       public List<Ticket> tickets= new List<Ticket>();
        public List<Project> projects = new List<Project>();
        public List<ApplicationUser> Owners = new List<ApplicationUser>();
    }
}
