using JelloTicket.BusinessLayer.Services;
using JelloTicket.BusinessLayer.ViewModels;
using JelloTicket.DataLayer.Data;
using JelloTicket.DataLayer.Models;
using JelloTicket.DataLayer.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticket = JelloTicket.DataLayer.Models.Ticket;

namespace UnitTests
{
    [TestClass]
    public class TicketBusinessLogicTests
    {
        public TicketBusinessLogic ticketBL { get; set; }
        public IQueryable<Ticket> data { get; set; }
        public IQueryable<ApplicationUser> users { get; set; }
        public IQueryable<TicketWatcher> ticketWatchers { get; set; }
        public IQueryable<Project> projects { get; set; }
        public IQueryable<UserProject> userProjects { get; set; }

        private Mock<IRepository<Ticket>> _ticketRepositoryMock;

        private Mock<IRepository<Project>> _projectRepositoryMock;

        private Mock<IRepository<Comment>> _commentRepositoryMock;


        //private static List<ApplicationUser> _users = new List<ApplicationUser>
        //{
        //    new ApplicationUser{UserName = "Jim",Email = "Jim@test.com" },
        //    new ApplicationUser{UserName = "Tom",Email = "Tom@test.com"},
        //    new ApplicationUser{UserName = "Attrain",Email = "Attrain@test.com"},
        //};
        //private  UserManager<ApplicationUser> _userManager = MockUserManager<ApplicationUser>(_users).Object;

        // spooky magic I did not write!
        // https://stackoverflow.com/questions/49165810/how-to-mock-usermanager-in-net-core-testing
        //public static Mock<UserManager<TUser>> MockUserManager<TUser>(List<TUser> ls) where TUser : class
        //{


        //    return mgr;
        //}

        [TestInitialize]
        public void Initialize()
        {


            List<ApplicationUser> CreatedUsers = new List<ApplicationUser>
            {
            new ApplicationUser {UserName = "Jim",Email = "Jim@test.com" },
            new ApplicationUser{UserName = "Tom",Email = "Tom@test.com"},
            new ApplicationUser{UserName = "Attrain",Email = "Attrain@test.com"},
            };
            users = CreatedUsers.AsQueryable();

            Mock<IUserStore<ApplicationUser>> store = new Mock<IUserStore<ApplicationUser>>();
            Mock<UserManager<ApplicationUser>> mgr = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<ApplicationUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<ApplicationUser>());

            mgr.Setup(x => x.DeleteAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Callback<ApplicationUser, string>((x, y) => CreatedUsers.Add(x));
            mgr.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);

            Mock<ApplicationDbContext> mockContext = new Mock<ApplicationDbContext>();

            Mock<DbSet<ApplicationUser>> mockUsers = new Mock<DbSet<ApplicationUser>>();

            mockUsers.As<IQueryable<ApplicationUser>>().Setup(m => m.Provider).Returns(users.Provider);
            mockUsers.As<IQueryable<ApplicationUser>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockUsers.As<IQueryable<ApplicationUser>>().Setup(m => m.Expression).Returns(users.Expression);
            mockUsers.As<IQueryable<ApplicationUser>>().Setup(m => m.GetEnumerator()).Returns(() => users.GetEnumerator());

            mockContext.Setup(t => t.Users).Returns(mockUsers.Object);

            //------------Ticket-----------
            List<Ticket> tasks = new List<Ticket>
            {
                new Ticket {Id = 1, Title = "Update User Profile", Body = "Implement functionality to update user profiles", RequiredHours = 8, Completed = false,Owner = CreatedUsers.FirstOrDefault(u=> u.UserName == "Jim") },
                new Ticket {Id = 2, Title = "Fix Login Bug", Body = "Investigate and fix the bug causing login issues for some users", RequiredHours = 6, Completed = true ,Owner = CreatedUsers.FirstOrDefault(u=> u.UserName == "Tom")},
                new Ticket {Id = 3, Title = "Implement Payment Gateway", Body = "Integrate a payment gateway to enable online transactions", RequiredHours = 10, Completed = false },
                new Ticket {Id = 4, Title = "Improve Search Algorithm", Body = "Optimize the search algorithm to provide faster and more accurate results", RequiredHours = 12, Completed = false }
            };
            data = tasks.AsQueryable();


            Mock<DbSet<Ticket>> mockTickets = new Mock<DbSet<Ticket>>();
            mockTickets.As<IQueryable<Ticket>>().Setup(m => m.Provider).Returns(data.Provider);
            mockTickets.As<IQueryable<Ticket>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockTickets.As<IQueryable<Ticket>>().Setup(m => m.Expression).Returns(data.Expression);
            mockTickets.As<IQueryable<Ticket>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            mockContext.Setup(t => t.Tickets).Returns(mockTickets.Object);


            List<TicketWatcher> watchers = new List<TicketWatcher>
            {new TicketWatcher {Id = 1,Watcher = CreatedUsers.Last(),Ticket = data.Last() }

            };
            ticketWatchers = watchers.AsQueryable();


            Mock<DbSet<TicketWatcher>> mockTicketWatchers = new Mock<DbSet<TicketWatcher>>();
            mockTicketWatchers.As<IQueryable<TicketWatcher>>().Setup(m => m.Provider).Returns(ticketWatchers.Provider);
            mockTicketWatchers.As<IQueryable<TicketWatcher>>().Setup(m => m.ElementType).Returns(ticketWatchers.ElementType);
            mockTicketWatchers.As<IQueryable<TicketWatcher>>().Setup(m => m.Expression).Returns(ticketWatchers.Expression);
            mockTicketWatchers.As<IQueryable<TicketWatcher>>().Setup(m => m.GetEnumerator()).Returns(() => ticketWatchers.GetEnumerator());

            mockContext.Setup(t => t.TicketWatchers).Returns(mockTicketWatchers.Object);



            List<Project> createProjects = new List<Project>
            {new Project {Id =1, ProjectName = "Project one ", CreatedBy = users.First(), },

            };
            projects = createProjects.AsQueryable();


            Mock<DbSet<Project>> mockProjects = new Mock<DbSet<Project>>();
            mockProjects.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(projects.Provider);
            mockProjects.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(projects.ElementType);
            mockProjects.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(projects.Expression);
            mockProjects.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(() => projects.GetEnumerator());

            mockContext.Setup(t => t.Projects).Returns(mockProjects.Object);



            List<UserProject> createdUserProjects = new List<UserProject>
            {new UserProject {Id =1, ApplicationUser = users.First(),Project = projects.First(),UserId = users.First().Id }
            };
            userProjects = createdUserProjects.AsQueryable();


            Mock<DbSet<UserProject>> mockuserProjects = new Mock<DbSet<UserProject>>();
            mockuserProjects.As<IQueryable<UserProject>>().Setup(m => m.Provider).Returns(userProjects.Provider);
            mockuserProjects.As<IQueryable<UserProject>>().Setup(m => m.ElementType).Returns(userProjects.ElementType);
            mockuserProjects.As<IQueryable<UserProject>>().Setup(m => m.Expression).Returns(userProjects.Expression);
            mockuserProjects.As<IQueryable<UserProject>>().Setup(m => m.GetEnumerator()).Returns(() => userProjects.GetEnumerator());

            mockContext.Setup(t => t.UserProjects).Returns(mockuserProjects.Object);




            Mock<IRepository<Ticket>> ticketRepositoryMock = new Mock<IRepository<Ticket>>();
            _ticketRepositoryMock = ticketRepositoryMock;

            Mock<IRepository<Project>> projectRepositoryMock = new Mock<IRepository<Project>>();
            _projectRepositoryMock = projectRepositoryMock;

            Mock<IRepository<Comment>> commentRepositoryMock = new Mock<IRepository<Comment>>();
            _commentRepositoryMock = commentRepositoryMock;

            Mock<UserManagerBusinessLogic> userManagerBusinessLogic = new Mock<UserManagerBusinessLogic>();
            Mock<UserProjectRepo> userProjectRepository = new Mock<UserProjectRepo>();
            Mock<IRepository<Ticket>> ticketRepository = new Mock<IRepository<Ticket>>();
            Mock<IUserProjectRepo> userProjectRepo = new Mock<IUserProjectRepo>();
            Mock<TicketWatcherRepo> ticketWatcherRepo = new Mock<TicketWatcherRepo>();

            ticketBL = new TicketBusinessLogic(new TicketRepo(mockContext.Object),
                     new ProjectRepo(mockContext.Object),
                    new CommentRepo(mockContext.Object),
                     mgr.Object,
                    new UserManagerBusinessLogic(mgr.Object),
                    new UserProjectRepo(mockContext.Object),
                       new TicketWatcherRepo(mockContext.Object), new UserRepo(mockContext.Object, mgr.Object)
            );

            //ticketRepositoryMock.Setup(tr => tr.Get(It.IsAny<int>()))
            //    .Returns((int ticketId) => data.FirstOrDefault(p => p.Id == ticketId));

        }
        [TestMethod]
        [DataRow(1)]

        public void GetTicketFromId_Solution(int id)
        {
            // Arrange 
            Ticket realTicket = data.FirstOrDefault(t => t.Id == id);

            // Act
            Ticket returnedTicket = ticketBL.GetTicketById(realTicket.Id);

            // Assert
            Assert.AreEqual(realTicket, returnedTicket);
        }

        [TestMethod]
        [DataRow(5)]
        public void GetTicketFromId_Problem(int id)
        {
            //id NULL
            Ticket realTicket = data.FirstOrDefault(t => t.Id == id);
            Assert.ThrowsException<NullReferenceException>(() => ticketBL.GetTicketById(realTicket?.Id));
        }

        [TestMethod]
        [DataRow(1)]
        public void GetUsersWithoutTicket_Solution(int id)
        {
            Ticket checkingTicket = data.FirstOrDefault(t => t.Id == id);

            IEnumerable<SelectListItem> user = ticketBL.users(checkingTicket);
            List<ApplicationUser> CheckcingUsers = users.ToList();
            Assert.AreNotEqual(CheckcingUsers, user);


        }

        [TestMethod]
        [DataRow(5)]
        public void GetUsersWithoutTicket_Problem(int id)
        {
            Ticket checkingTicket = data.FirstOrDefault(t => t.Id == id);


            Assert.ThrowsException<NullReferenceException>(() => ticketBL.users(checkingTicket));


        }

        [TestMethod]
        [DataRow(1)]
        public void DoesTicketExist_Solution(int id)
        {

            bool exisitngticket = ticketBL.DoesTicketExist(id);

            Assert.IsTrue(exisitngticket);

        }
        [TestMethod]
        [DataRow(null)]
        public void DoesTicketExist_NullId(int? id)
        {


            Assert.ThrowsException<NullReferenceException>(() => ticketBL.DoesTicketExist(id));
        }
        [TestMethod]
        public void AddtoWatcher_solution()
        {
            Ticket checkingTicket = data.FirstOrDefault(t => t.Id == 1);
            ApplicationUser user1 = users.First();

            ticketBL.AddtoWatchers(user1.UserName, checkingTicket.Id);

            Assert.AreEqual(checkingTicket.TicketWatchers.First(), user1.TicketWatching.First());
        }

        [TestMethod]
        public void AddtoWatcher_Problem()
        {
            Ticket checkingTicket = data.FirstOrDefault(t => t.Id == 1);

            Assert.ThrowsException<NullReferenceException>(() => ticketBL.AddtoWatchers(null, checkingTicket.Id));
        }
        [TestMethod]
        public void UnWatch_solution()
        {
            Ticket checkingTicket = data.Last();
            ApplicationUser user1 = users.Last();
            ticketBL.AddtoWatchers(user1.UserName, checkingTicket.Id);

            ticketBL.UnWatch(user1.UserName, checkingTicket.Id);

            Assert.AreEqual(checkingTicket.TicketWatchers.First(), user1.TicketWatching.First());
        }

        [TestMethod]
        public void UnWatch_Problem()
        {
            Ticket checkingTicket = data.FirstOrDefault(t => t.Id == 1);

            Assert.ThrowsException<NullReferenceException>(() => ticketBL.UnWatch(null, checkingTicket.Id));
        }
        [TestMethod]
        public void CreatePost_Solution()
        {
            Project project = projects.First();
            ApplicationUser user = users.First();
            Ticket ticket = data.Last();
            ticketBL.CreatePost(project.Id, user.Id, ticket);
            Assert.AreEqual(user, ticket.Owner);



        }
        [TestMethod]

        public void CreateGet_Solution()
        {
            Project project = projects.First();
          TicketCreateVM result =   ticketBL.CreateGet(project.Id);
            Assert.AreEqual(project, result.project);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        [DataRow(null)]
        public void CreateGet_Problem(int ? id)
        {
          
            Assert.ThrowsException<NullReferenceException>(() => ticketBL.CreateGet(id) );

        }

    
        [TestMethod]
        [DataRow(null)] 
        public void RemoveUser_Problem(string? id)
        {
            Ticket ticket = data.First();
            Assert.ThrowsException<ArgumentException>(() => ticketBL.RemoveUser(id, ticket.Id));
        }

            [TestMethod]
        public void RemoveUser_Solution()
        {
            Ticket ticket = data.First();
            ApplicationUser user = users.First();

            ticketBL.RemoveUser(user.Id, ticket.Id);

            Assert.IsNull(ticket.Owner);

        }
        
    
    
    }

}
