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

        public Ticket TicketEdit(int? id)
        {
            if (id == null)
            {
                throw new NullReferenceException("Id is NUll");
            }
            else
            {
                Ticket ticket = _ticketRepo.Get(id);
                if(ticket == null)
                {
                    throw new Exception("ticket with given id is not found");
                }else
                {
                    List<ApplicationUser> results = _users.Users.Where(u => u != ticket.Owner).ToList();

                    List<TicketEditVM> currUsers = new List<TicketEditVM>();

                    results.ForEach(r =>
                    {
                    TicketEditVM vm = new TicketEditVM();
                        vm.UserName= r.UserName;
                        vm.Editedid = r.Id;
                        currUsers.Add(vm);
                    });

                    return ticket;

                }
            }
        }



    }


}
