using JelloTicket.DataLayer.Data;
using JelloTicket.DataLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;

namespace SD_340_W22SD_Final_Project_Group6.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _users;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> users)
        {
            _context = context;
            _users = users;
        }
        public async Task<IActionResult> Index()
        {
            ProjectManagersAndDevelopersViewModels vm = new ProjectManagersAndDevelopersViewModels();

            List<ApplicationUser> pmUsers = (List<ApplicationUser>)await _users.GetUsersInRoleAsync("ProjectManager");
            List<ApplicationUser> devUsers = (List<ApplicationUser>)await _users.GetUsersInRoleAsync("Developer");
            List<ApplicationUser> allUsers = _context.Users.ToList();



            //vm.pms = (ICollection<Models.ApplicationUser>)pmUsers;
            //vm.devs = (ICollection<Models.ApplicationUser>)devUsers;
            //vm.allUsers = (ICollection<Models.ApplicationUser>)allUsers;
            return View(vm);
        }

        public async Task<IActionResult> ReassignRoleAsync()
        {
            List<ApplicationUser> allUsers = _context.Users.ToList();

            List<SelectListItem> users = new List<SelectListItem>();
            allUsers.ForEach(u =>
            {
                users.Add(new SelectListItem(u.UserName, u.Id.ToString()));
            });
            ViewBag.Users = users;

            return View(allUsers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReassignRole(string role, string userId)
        {

            ApplicationUser user = _users.Users.First(u => u.Id == userId);
            ICollection<string> roleUser = await _users.GetRolesAsync(user);
            if (roleUser.Count == 0)
            {
                await _users.AddToRoleAsync(user, role);
                return RedirectToAction("Index", "Admin", new { area = "" });
            } else
            {
                await _users.RemoveFromRoleAsync(user, roleUser.First());
                await _users.AddToRoleAsync(user, role);
                return RedirectToAction("Index", "Admin", new { area = "" });
            }
        }
    }
}

