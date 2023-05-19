using JelloTicket.BusinessLayer.Services;
using JelloTicket.DataLayer.Models;
using JelloTicket.DataLayer.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JelloTicket.DataLayer.Repositories;
using Microsoft.AspNetCore.Identity;
using JelloTicket.BusinessLayer.ViewModels;

namespace SD_340_W22SD_Final_Project_Group6.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly TicketBusinessLogic ticketBusinessLogic;
        private readonly ProjectBusinessLogic projectBusinessLogic;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IRepository<Comment> _commentRepository;
        private readonly IRepository<UserProject> _userprojectRepo;
        private readonly IRepository<TicketWatcher> _ticketWatcher;

        public TicketsController(IRepository<Ticket> ticketRepo, IRepository<Project> projectRepo, UserManager<ApplicationUser> userManager, IRepository<Comment> commentRepository, IRepository<UserProject> userprojectRepo, IRepository<TicketWatcher> ticketWatcher)
        {
            ticketBusinessLogic = new TicketBusinessLogic(ticketRepo, projectRepo, userManager, commentRepository, userprojectRepo, ticketWatcher);
            _commentRepository = commentRepository;
            _ticketWatcher = ticketWatcher;
        }

        // GET: Tickets
        public IResult Index()
        {
            return ticketBusinessLogic.GetTickets();
        }
        /*
            // GET: Tickets/Details/5
            public async Task<IActionResult> Details(int? id)
            {
                if (id == null || _context.Tickets == null)
                {
                    return NotFound();
                }

                var ticket = await _context.Tickets.Include(t => t.Project).Include(t => t.TicketWatchers).ThenInclude(tw => tw.Watcher).Include(u => u.Owner).Include(t => t.Comments).ThenInclude(c => c.CreatedBy)
                    .FirstOrDefaultAsync(m => m.Id == id);
                List<SelectListItem> currUsers = new List<SelectListItem>();
                ticket.Project.AssignedTo.ToList().ForEach(t =>
                {
                    currUsers.Add(new SelectListItem(t.ApplicationUser.UserName, t.ApplicationUser.Id.ToString()));
                });
                ViewBag.Users = currUsers;

                if (ticket == null)
                {
                    return NotFound();
                }

                return View(ticket);
            }
        */
        // GET: Tickets/Create
        [Authorize(Roles = "ProjectManager")]
        public IActionResult Create(int projId)
        {

            TicketCreateVM vm = ticketBusinessLogic.CreateGet(projId);
            return View(vm);

        }


        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Create([Bind("Id,Title,Body,RequiredHours,TicketPriority")] Ticket ticket, int projId, string userId)
        {
            if (ModelState.IsValid)
            {

                ticketBusinessLogic.CreatePost(projId, userId, ticket);
                return RedirectToAction("Index", "Projects", new { area = "" });
            }
            return View(ticket);
        }


        // GET: Tickets/Edit/5
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Edit(int? id)
        {
            Ticket ticket = ticketBusinessLogic.GetTicketById(id);
            TicketEditVM vm = ticketBusinessLogic.EditGet(ticket);
            return View(vm);

        }



        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Edit(int id, string userId, TicketEditVM ticketVM)
        {
            ticketBusinessLogic.EditTicket(ticketVM, id, userId);

            return RedirectToAction(nameof(Edit), new { id = ticketVM.ticket.Id });

        }


        #region NOt need now
        /*
        [Authorize(Roles = "ProjectManager")]
            public async Task<IActionResult> RemoveAssignedUser(string id, int ticketId)
            {
                if (id == null)
                {
                    return NotFound();
                }
                Ticket currTicket = await _context.Tickets.Include(t => t.Owner).FirstAsync(t => t.Id == ticketId);
                ApplicationUser currUser = await _context.Users.FirstAsync(u => u.Id == id);
                //To be fixed ASAP
                currTicket.Owner = currUser;
                await _context.SaveChangesAsync();

                return RedirectToAction("Edit", new { id = ticketId });
            }

          
            [HttpPost]
            public async Task<IActionResult> CommentTask(int TaskId, string? TaskText)
            {
                if (TaskId != null || TaskText != null)
                {
                    try
                    {
                        Comment newComment = new Comment();
                        string userName = User.Identity.Name;
                        ApplicationUser user = _context.Users.First(u => u.UserName == userName);
                        Ticket ticket = _context.Tickets.FirstOrDefault(t => t.Id == TaskId);

                        newComment.CreatedBy = user;
                        newComment.Description = TaskText;
                        newComment.Ticket = ticket;
                        user.Comments.Add(newComment);
                        _context.Comments.Add(newComment);
                        ticket.Comments.Add(newComment);

                        int Id = TaskId;
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Details", new {Id});

                    }
                    catch (Exception ex)
                    {
                        return RedirectToAction("Error", "Home");
                    }
                }
                return RedirectToAction("Index");
            }

            public async Task<IActionResult> UpdateHrs(int id, int hrs)
            {
                if (id != null || hrs != null)
                {
                    try
                    {
                        Ticket ticket = _context.Tickets.FirstOrDefault(t => t.Id == id);
                        ticket.RequiredHours = hrs;
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Details", new { id });

                    }
                    catch (Exception ex)
                    {
                        return RedirectToAction("Error", "Home");
                    }
                }
                return RedirectToAction("Index");
            }
        */
        public async Task<IActionResult> AddToWatchers(int id)
        {

            string userName = User.Identity.Name;
            ticketBusinessLogic.AddtoWatchers(userName, id);
            return RedirectToAction("Details", new { id });

        }

        public async Task<IActionResult> UnWatch(int id)
        {
            string userName = User.Identity.Name;
            ticketBusinessLogic.UnWatch(userName, id);
            return RedirectToAction("Details", new { id });

        }
        /*
            public async Task<IActionResult> MarkAsCompleted(int id)
            {
                if (id != null)
                {
                    try
                    {
                        Ticket ticket = _context.Tickets.FirstOrDefault(t => t.Id == id);
                        ticket.Completed = true;

                        await _context.SaveChangesAsync();
                        return RedirectToAction("Details", new { id });

                    }
                    catch (Exception ex)
                    {
                        return RedirectToAction("Error", "Home");
                    }
                }
                return RedirectToAction("Index");
            }

            public async Task<IActionResult> UnMarkAsCompleted(int id)
            {
                if (id != null)
                {
                    try
                    {
                        Ticket ticket = _context.Tickets.FirstOrDefault(t => t.Id == id);
                        ticket.Completed = false;

                        await _context.SaveChangesAsync();
                        return RedirectToAction("Details", new { id });

                    }
                    catch (Exception ex)
                    {
                        return RedirectToAction("Error", "Home");
                    }
                }
                return RedirectToAction("Index");
            }


            // GET: Tickets/Delete/5
            [Authorize(Roles = "ProjectManager")]
            public async Task<IActionResult> Delete(int? id)
            {
                if (id == null || _context.Tickets == null)
                {
                    return NotFound();
                }

                var ticket = await _context.Tickets.Include(t => t.Project)
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (ticket == null)
                {
                    return NotFound();
                }

                return View(ticket);
            }

            // POST: Tickets/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            [Authorize(Roles = "ProjectManager")]
            public async Task<IActionResult> DeleteConfirmed(int id, int projId)
            {
                if (_context.Tickets == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Tickets'  is null.");
                }
                var ticket = await _context.Tickets.Include(t => t.Project).FirstAsync(p => p.Id == id);
                Project currProj = await _context.Projects.FirstAsync(p => p.Id.Equals(projId));
                if (ticket != null)
                {
                    currProj.Tickets.Remove(ticket);
                    _context.Tickets.Remove(ticket);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Projects");
            }

            private bool TicketExists(int id)
            {
              return (_context.Tickets?.Any(e => e.Id == id)).GetValueOrDefault();
            }
        */
        #endregion
    }
}

