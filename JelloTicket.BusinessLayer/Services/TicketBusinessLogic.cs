using JelloTicket.BusinessLayer.ViewModels;
using JelloTicket.DataLayer.Models;
using JelloTicket.DataLayer.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        private readonly IRepository<Ticket> _ticketRepo;
        private readonly IRepository<Project> _projectRepo;
        private readonly UserManager<ApplicationUser> _users;
        private IRepository<Ticket> ticketRepo;

        public TicketBusinessLogic(IRepository<Ticket> ticketRepo, IRepository<Project> projectRepo, UserManager<ApplicationUser> users)
        {
            _ticketRepo = ticketRepo;
            _projectRepo = projectRepo;
            _users = users;
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

        public Ticket GetTicketById(int? id)
        {
            if (id == null)
            {
                throw new NullReferenceException("Id is NUll");
            }
            else
            {
                Ticket ticket = _ticketRepo.Get(id);
                if (ticket == null)
                {
                    throw new Exception("ticket with given id is not found");
                }
                else
                {
                

                    // Only return the Ticket for the HTTP GET method
                    return ticket;

                }
            }
        }

        public IEnumerable<SelectListItem> users(Ticket ticket)
        {
            IEnumerable<SelectListItem> currUsers = _users.Users.Where(u => u != ticket.Owner);
            return currUsers;
        }
        // forum submission is taken and submitted to db
        public TicketEditVM EditTicket(TicketEditVM ticketVM,int id, string userId)
        {
            if (id != ticketVM.ticket.Id)
            {
                throw new Exception("Not Found");
            }
            ApplicationUser currUser = _users.Users.FirstOrDefault(u => u.Id == userId);
            ticketVM.ticket.Owner = currUser;
            // business logic for editing the ticket here
            _ticketRepo.Update(ticketVM.ticket);
            return ticketVM;
        }


    }


}
