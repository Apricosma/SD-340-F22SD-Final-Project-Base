using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JelloTicket.DataLayer.Repositories;
using JelloTicket.DataLayer.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JelloTicket.BusinessLayer.HelperLibrary;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Project = JelloTicket.DataLayer.Models.Project;
using JelloTicket.BusinessLayer.ViewModels;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace JelloTicket.BusinessLayer.Services
{
    public class TicketBusinessLogic
    {
        private readonly IRepository<Ticket> _ticketRepository;
        private readonly IRepository<DataLayer.Models.Project> _projectRepository;
        private readonly IRepository<Comment> _commentRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HelperMethods _helperMethods;
        private readonly UserManagerBusinessLogic _userManagerBusinessLogic;
        private readonly IRepository<UserProject> _userProjectRepo;
        private readonly IRepository<TicketWatcher> _ticketWatcher;
        private readonly IRepository<ApplicationUser> _userRepo;

        public TicketBusinessLogic(IRepository<Ticket> ticketRepository
            , IRepository<DataLayer.Models.Project> projectRepository
            , IRepository<Comment> commentRepository
            , UserManager<ApplicationUser> userManager
            , UserManagerBusinessLogic userManagerBusinessLogic
            , IRepository<UserProject> userProjectRepo
            , IRepository<TicketWatcher> ticketWatcher, IRepository<ApplicationUser> UserRepo)
        {
            _ticketRepository = ticketRepository;
            _projectRepository = projectRepository;
            _commentRepository = commentRepository;
            _userManager = userManager;
            _userManagerBusinessLogic = userManagerBusinessLogic;
            _userProjectRepo = userProjectRepo;
            _ticketWatcher = ticketWatcher;
            _userRepo = UserRepo;
        }

        public Ticket GetTicketById(int? id)
        {
            if (id == null)
            {
                throw new NullReferenceException("Id is NUll");
            }
            else
            {
                Ticket ticket = _ticketRepository.Get(id);
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

        public async Task<Ticket> GetTicketDetails(int? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            Ticket? ticket = _ticketRepository.Get(id);

            return (ticket);
        }

        public void RemoveUser(string userId, int ticketId)
        {
            if (userId == null)
            {
                throw new ArgumentException(nameof(userId));
            }

            Ticket currentTicket = _ticketRepository.Get(ticketId);

            List<ApplicationUser> Users = _userRepo.GetAll().ToList();
            ApplicationUser applicationUser = Users.FirstOrDefault(u => u.Id == userId);

            if (currentTicket != null && applicationUser != null)
            {
                Users.Remove(applicationUser);

                currentTicket.Owner = null;
            }
        }

        public void Save()
        {
            _ticketRepository.Save();
        }

        public void RemoveTicket(Ticket ticket)
        {
            int ticketId = ticket.Id;
            _ticketRepository.Delete(ticketId);
        }

        public bool DoesTicketExist(int? id)
        {
            if (id == null)
            {
                throw new NullReferenceException("Id is NUll");
            }

            return _ticketRepository.Exists(id);
        }

        public TicketCreateVM CreateGet(int? projId)
        {
            if (projId == null)
            {
                throw new NullReferenceException("Id is invalid");
            }
            else
            {
                Project currentProject = _projectRepository.Get(projId);

                if (currentProject == null)
                {
                    throw new Exception("Cannot find project with the given id ");
                }
                else
                {
                    TicketCreateVM vm = new TicketCreateVM();
                    UserProject userProject = _userProjectRepo.Get(projId);

                    ICollection<UserProject> projects = _userProjectRepo.GetAll();

                    List<string> projectUserIds = projects.Select(p => p.UserId).ToList();

                    List<ApplicationUser> Users = _userRepo.GetAll().ToList();

                    List<ApplicationUser> users = Users.Where(u => projectUserIds.Contains(u.Id)).ToList();

                    currentProject.AssignedTo = projects;

                    int index = 0;
                    foreach (UserProject project in projects)
                    {
                        if (project.ProjectId == projId)
                        {
                            project.ApplicationUser = users[index];
                            index++;
                            if (index >= users.Count)
                            {
                                index = 0;
                            }
                        }
                    }

                    List<SelectListItem> currUsers = new List<SelectListItem>();
                    currentProject.AssignedTo.ToList().ForEach(t =>
                    {
                        currUsers.Add(new SelectListItem(t.ApplicationUser.UserName, t.ApplicationUser.Id.ToString()));
                    });

                    vm.project = currentProject;
                    vm.currUsers = currUsers;
                    return vm;

                }
            }

        }

        public Ticket CreatePost(int projId, string userId, Ticket ticket)
        {

            if (projId == null || userId == null|| ticket == null)
            {
                throw new NullReferenceException("arguments cannot be null");
            }

            Project currProj = _projectRepository.Get(projId);
            ticket.Project = currProj;


            List<ApplicationUser> Users = _userRepo.GetAll().ToList();

            ApplicationUser owner = Users.FirstOrDefault(u => u.Id == userId);
            ticket.Owner = owner;
            _ticketRepository.Create(ticket);
            currProj.Tickets.Add(ticket);
            return ticket;

        }
        public ICollection<Ticket> GetTickets()
        {
            List<Ticket> tickets = _ticketRepository.GetAll().ToList();
            return tickets;
        }

        public TicketEditVM EditGet(Ticket ticket)
        {
            TicketEditVM vm = new TicketEditVM();
            vm.ticket = ticket;
            IEnumerable<SelectListItem> remainingUsers = users(ticket);
            vm.Users = remainingUsers;
            return vm;
        }
        //public Ticket GetTicketById(int? id)
        //{
        //    if (id == null)
        //    {
        //        throw new NullReferenceException("Id is NUll");
        //    }
        //    else
        //    {
        //        Ticket ticket = _ticketRepository.Get(id);
        //        if (ticket == null)
        //        {
        //            throw new Exception("ticket with given id is not found");
        //        }
        //        else
        //        {
        //            // Only return the Ticket for the HTTP GET method
        //            return ticket;
        //        }
        //    }
        //}

        public IEnumerable<SelectListItem> users(Ticket ticket)
        {
            if(ticket == null)
            {
                throw new NullReferenceException("Provided Ticket is Null");
            }

            List<ApplicationUser> Users = _userRepo.GetAll().ToList();

            List<ApplicationUser> results = Users.Where(u => u != ticket.Owner).ToList();

            List<SelectListItem> currUsers = new List<SelectListItem>();
            results.ForEach(r =>
            {
                currUsers.Add(new SelectListItem(r.UserName, r.Id.ToString()));
            }); return currUsers;
        }
        // forum submission is taken and submitted to db
        public TicketEditVM EditTicket(TicketEditVM ticketVM, int id, string userId)
        {
            if (id != ticketVM.ticket.Id)
            {
                throw new Exception("Not Found");
            }            List<ApplicationUser> Users = _userRepo.GetAll().ToList();

            ApplicationUser currUser = _userManager.Users.FirstOrDefault(u => u.Id == userId);
            ticketVM.ticket.Owner = currUser;
            // business logic for editing the ticket here
            _ticketRepository.Update(ticketVM.ticket);
            return ticketVM;
        }

        public void AddtoWatchers(string userName, int id)
        {

            TicketWatcher newTickWatch = new TicketWatcher();
            if (userName == null)
            {
                throw new NullReferenceException("UserName is empty:");
            }
            List<ApplicationUser> Users = _userRepo.GetAll().ToList();

            ApplicationUser user = Users.FirstOrDefault(u => u.UserName == userName);
            Ticket ticket = _ticketRepository.Get(id);
            newTickWatch.Ticket = ticket;
            newTickWatch.Watcher = user;
            user.TicketWatching.Add(newTickWatch);
            ticket.TicketWatchers.Add(newTickWatch);
            _ticketWatcher.Create(newTickWatch);


        }
        public void UnWatch(string userName, int id)
        {
            List<ApplicationUser> Users = _userRepo.GetAll().ToList();

            ApplicationUser user = Users.FirstOrDefault(u => u.UserName == userName);
            if(user == null )
            {
                throw new NullReferenceException(" user Cannot be found");
            }
 
            Ticket ticket = _ticketRepository.Get(id);
            List<TicketWatcher> ticketWatchers = _ticketWatcher.GetAll().ToList();
            TicketWatcher currTickWatch = ticketWatchers.First(tw => tw.Ticket == ticket && tw.Watcher == user);

            _ticketWatcher.Delete(currTickWatch.Id);
            ticket.TicketWatchers.Remove(currTickWatch);
            user.TicketWatching.Remove(currTickWatch);

        }



    }
}
