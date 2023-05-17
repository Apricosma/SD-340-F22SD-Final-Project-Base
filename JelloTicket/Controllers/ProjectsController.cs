using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using JelloTicket.DataLayer.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Mvc;
using JelloTicket.DataLayer.Data;
using JelloTicket.DataLayer.Models;

namespace SD_340_W22SD_Final_Project_Group6.Controllers
{
    [Authorize(Roles = "ProjectManager, Developer")]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _users;
        //private readonly IRepository<Project> _projectRepo;

        public ProjectsController(ApplicationDbContext context, UserManager<ApplicationUser> users)
        {
            _context = context;
            _users = users;
        }
        // GET: Projects
        [Authorize]
        public async Task<IActionResult> Index(string? sortOrder, int? page, bool? sort, string? userId)
        {
            List<Project> SortedProjs = new List<Project>();
            List<ApplicationUser> allUsers = (List<ApplicationUser>)await _users.GetUsersInRoleAsync("Developer");

            List<SelectListItem> users = new List<SelectListItem>();
            allUsers.ForEach(au =>
            {
                users.Add(new SelectListItem(au.UserName, au.Id.ToString()));
            });
            ViewBag.Users = users;

            var query = _context.Projects
                .Include(p => p.CreatedBy)
                .Include(p => p.AssignedTo)
                    .ThenInclude(at => at.ApplicationUser)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.Owner)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.TicketWatchers)
                    .ThenInclude(tw => tw.Watcher);

            var projects = await query.ToListAsync();

            foreach (var project in projects)
            {
                switch (sortOrder)
                {
                    case "Priority":
                        project.Tickets = sort == true
                            ? project.Tickets.OrderByDescending(t => t.TicketPriority).ToList()
                            : project.Tickets.OrderBy(t => t.TicketPriority).ToList();
                        break;
                    case "RequiredHrs":
                        project.Tickets = sort == true
                            ? project.Tickets.OrderByDescending(t => t.RequiredHours).ToList()
                            : project.Tickets.OrderBy(t => t.RequiredHours).ToList();
                        break;
                    case "Completed":
                        project.Tickets = project.Tickets.Where(t => (bool)t.Completed).ToList();
                        break;
                    default:
                        // No sorting for tickets within the project
                        break;
                }

            }
            
            //check if User is PM or Develoer
            var LogedUserName = User.Identity.Name;  // logined user name
            var user = _context.Users.FirstOrDefault(u => u.UserName == LogedUserName);
            var rolenames = await _users.GetRolesAsync(user);
            var AssinedProject = new List<Project>();
            // geting assined project
            if (rolenames.Contains("Developer"))
            {
                AssinedProject = projects.Where(p => p.AssignedTo.Select(projectUser => projectUser.UserId).Contains(user.Id)).ToList();
            }
            else
            {
                AssinedProject = projects;
            }
            X.PagedList.IPagedList<Project> projList = AssinedProject.ToPagedList(page ?? 1, 3);
            return View(projList);
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        public async Task<IActionResult> RemoveAssignedUser(string id, int projId)
        {
            if (id == null)
            {
                return NotFound();
            }
            UserProject currUserProj = await _context.UserProjects.FirstAsync(up => up.ProjectId == projId && up.UserId == id);
            _context.UserProjects.Remove(currUserProj);
            await _context.SaveChangesAsync();

            return RedirectToAction("Edit", new { id = projId });
        }

        // GET: Projects/Create
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> CreateAsync()
        {
            List<ApplicationUser> allUsers = (List<ApplicationUser>)await _users.GetUsersInRoleAsync("Developer");

            List<SelectListItem> users = new List<SelectListItem>();
            allUsers.ForEach(au =>
            {
                users.Add(new SelectListItem(au.UserName, au.Id.ToString()));
            });
            ViewBag.Users = users;

            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Create([Bind("Id,ProjectName")] Project project, List<string> userIds)
        {
            if (ModelState.IsValid)
            {
                string userName = User.Identity.Name;

                ApplicationUser createdBy = _context.Users.First(u => u.UserName == userName);
                userIds.ForEach((user) =>
                {
                    ApplicationUser currUser = _context.Users.FirstOrDefault(u => u.Id == user);
                    UserProject newUserProj = new UserProject();
                    newUserProj.ApplicationUser = currUser;
                    newUserProj.UserId = currUser.Id;
                    newUserProj.Project = project;
                    project.AssignedTo.Add(newUserProj);
                    _context.UserProjects.Add(newUserProj);
                });
                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.Include(p => p.AssignedTo).FirstAsync(p => p.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            List<ApplicationUser> results = _context.Users.ToList();

            List<SelectListItem> currUsers = new List<SelectListItem>();
            results.ForEach(r =>
            {
                currUsers.Add(new SelectListItem(r.UserName, r.Id.ToString()));
            });
            ViewBag.Users = currUsers;

            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Edit(int id, List<string> userIds, [Bind("Id,ProjectName")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    userIds.ForEach((user) =>
                    {
                        ApplicationUser currUser = _context.Users.FirstOrDefault(u => u.Id == user);
                        UserProject newUserProj = new UserProject();
                        newUserProj.ApplicationUser = currUser;
                        newUserProj.UserId = currUser.Id;
                        newUserProj.Project = project;
                        project.AssignedTo.Add(newUserProj);
                    });
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Edit), new { id = id });
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Projects == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Projects'  is null.");
            }
            var project = await _context.Projects.Include(p => p.Tickets).FirstAsync(p => p.Id == id);
            if (project != null)
            {
                List<Ticket> tickets = project.Tickets.ToList();
                tickets.ForEach(ticket =>
                {
                    _context.Tickets.Remove(ticket);
                });
                await _context.SaveChangesAsync();
                List<UserProject> userProjects = _context.UserProjects.Where(up => up.ProjectId == project.Id).ToList();
                userProjects.ForEach(userProj =>
                {
                    _context.UserProjects.Remove(userProj);
                });

                _context.Projects.Remove(project);


            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return (_context.Projects?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
