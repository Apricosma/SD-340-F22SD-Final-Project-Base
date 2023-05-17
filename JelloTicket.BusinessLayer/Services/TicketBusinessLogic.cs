using JelloTicket.BusinessLayer.ViewModels;
using JelloTicket.DataLayer.Models;
using JelloTicket.DataLayer.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JelloTicket.BusinessLayer.Services
{
    public class TicketBusinessLogic
    {

        private readonly TicketRepo _ticketRepo;
        private readonly ProjectRepo _projectRepo;
        public TicketBusinessLogic(TicketRepo ticketRepo, ProjectRepo projectRepo)
        {
            _ticketRepo = ticketRepo;
            _projectRepo = projectRepo;
        }



        public ICollection<Ticket> GetTickets()
        {

            List<Ticket> list = _ticketRepo.GetAll().ToList();
            if (list.Count == 0)
            {
                throw new NullReferenceException("tickets are Empty");
            }
            else
            {
                return _ticketRepo.GetAll();

            }


        }

        public TicketVM Details(int? id)
        {
            if (id == null)
            {
                throw new NullReferenceException(" Ticket id is null in Details");
            }
            else
            {

                Ticket ticket = _ticketRepo.Get(id);
                Project project = ticket.Project;
                

                if (ticket == null)
                {
                    throw new NullReferenceException("Cannot Find Ticket with the given id ");
                }
                else
                {
             
                    TicketVM vm = new TicketVM();
                    vm.Tickets.Add(ticket);
                    vm.Project = project;
                    vm.TicketWatcher = ticket.TicketWatchers.ToList();

                    TicketWatcher ticketWatcher = ticket.TicketWatchers.FirstOrDefault(tw => tw.Ticket.Id == id);
                    vm.Watcher = ticketWatcher.Watcher;
                    vm.Owner = ticket.Owner;
                    vm.Comment = ticket.Comments.ToList();
                    Comment comment = ticket.Comments.FirstOrDefault(c => c.Ticket.Id == id);
                    vm.CreatedBy = comment.CreatedBy;
                    return vm;

                }
            }
        }

    }


}
