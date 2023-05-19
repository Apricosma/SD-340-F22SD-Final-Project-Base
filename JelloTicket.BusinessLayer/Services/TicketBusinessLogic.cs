using JelloTicket.BusinessLayer.ViewModels;
using JelloTicket.DataLayer.Models;
using JelloTicket.DataLayer.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace JelloTicket.BusinessLayer.Services
{
	public class TicketBusinessLogic
	{

		private readonly IRepository<Ticket> _ticketRepo;
		private readonly IRepository<Project> _projectRepo;
		private readonly IRepository<Comment> _commentRepo;
		private readonly UserManager<ApplicationUser> _users;
		private readonly IRepository<UserProject> _userProjectRepo;
		private readonly IRepository<TicketWatcher> _ticketWatcher;


		private IRepository<Ticket> ticketRepo;

		public TicketBusinessLogic(IRepository<Ticket> ticketRepo, IRepository<Project> projectRepo, UserManager<ApplicationUser> users, IRepository<Comment> commentRepo, IRepository<UserProject> userProjectRepo, IRepository<TicketWatcher> ticketWatcher)
		{
			_ticketRepo = ticketRepo;
			_projectRepo = projectRepo;
			_users = users;
			_commentRepo = commentRepo;
			_userProjectRepo = userProjectRepo;
			_ticketWatcher = ticketWatcher;
		}

		public TicketCreateVM CreateGet(int projId)
		{
			if (projId == null)
			{
				throw new Exception("Id is invalid");
			}
			else
			{
				Project currentProject = _projectRepo.Get(projId);

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

					List<ApplicationUser> users = _users.Users.Where(u => projectUserIds.Contains(u.Id)).ToList();

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
			Project currProj = _projectRepo.Get(projId);
			ticket.Project = currProj;
			ApplicationUser owner = _users.Users.FirstOrDefault(u => u.Id == userId);
			ticket.Owner = owner;
			_ticketRepo.Create(ticket);
			currProj.Tickets.Add(ticket);
			return ticket;

		}

		public void AddtoWatchers(string userName, int id)
		{

			TicketWatcher newTickWatch = new TicketWatcher();
			if(userName== null)
			{
				throw new Exception("UserName is empty:");
			}
			ApplicationUser user = _users.Users.First(u => u.UserName == userName);
            Ticket ticket = _ticketRepo.Get(id);
            newTickWatch.Ticket = ticket;
			newTickWatch.Watcher = user;
			user.TicketWatching.Add(newTickWatch);
			ticket.TicketWatchers.Add(newTickWatch);
			_ticketWatcher.Create(newTickWatch);


		}
		public void UnWatch(string userName,int id) 
		{
            ApplicationUser user = _users.Users.First(u => u.UserName == userName);
            Ticket ticket = _ticketRepo.Get(id);
			List<TicketWatcher> ticketWatchers = _ticketWatcher.GetAll().ToList();
            TicketWatcher currTickWatch =ticketWatchers.First(tw => tw.Ticket.Equals(ticket) && tw.Watcher.Equals(user));

            _ticketRepo.Delete(currTickWatch.Id);
            ticket.TicketWatchers.Remove(currTickWatch);
            user.TicketWatching.Remove(currTickWatch);

        }
        public IResult GetTickets()
		{

			List<Ticket> tickets = _ticketRepo.GetAll().ToList();

			foreach (Ticket ticket in tickets)
			{
				int projId = ticket.ProjectId;
				Project project = _projectRepo.GetAll().FirstOrDefault(p => p.Id == projId);
				ticket.Project = project;
				List<Comment> comments = _commentRepo.GetAll().Where(c => c.TicketId == ticket.Id).ToList();

				ticket.Comments = comments;
			}
			JsonSerializerOptions options = new JsonSerializerOptions
			{
				ReferenceHandler = ReferenceHandler.Preserve,
				MaxDepth = 0
			};


			string jsonResult = JsonSerializer.Serialize(tickets, options);
			Object jsonObject = JsonSerializer.Deserialize<object>(jsonResult);

			return Results.Ok(jsonObject);


		}

		public TicketEditVM EditGet(Ticket ticket)
		{
			TicketEditVM vm = new TicketEditVM();
			vm.ticket = ticket;
			IEnumerable<SelectListItem> remainingUsers = users(ticket);
			vm.Users = remainingUsers;
			return vm;
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
			List<ApplicationUser> results = _users.Users.Where(u => u != ticket.Owner).ToList();

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
			}
			ApplicationUser currUser = _users.Users.FirstOrDefault(u => u.Id == userId);
			ticketVM.ticket.Owner = currUser;
			// business logic for editing the ticket here
			_ticketRepo.Update(ticketVM.ticket);
			return ticketVM;
		}


	}


}
