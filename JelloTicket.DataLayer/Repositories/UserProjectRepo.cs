using JelloTicket.DataLayer.Data;
using JelloTicket.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JelloTicket.DataLayer.Repositories
{
    public class UserProjectRepo : IRepository<UserProject>, IUserProjectRepo
    {
        private readonly ApplicationDbContext _context;

        public UserProjectRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public UserProjectRepo() { }

        public virtual UserProject Get(int? id)
        {
            return (UserProject)_context.UserProjects.First(t => t.Id == id);
        }

        public virtual ICollection<UserProject> GetAll()
        {
            return _context.UserProjects.ToHashSet();
        }

        public void Create(UserProject up)
        {
            _context.Add(up);
            _context.SaveChanges();
        }

        public virtual void Update(UserProject up)
        {
            _context.Update(up);
            _context.SaveChanges();
        }

        public void Delete(int? id)
        {
            _context.UserProjects.Remove(_context.UserProjects.First(t => t.Id == id));
            _context.SaveChanges();
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public bool Exists(int? id)
        {
            return _context.UserProjects.Any(up => up.Id == id);
        }

        public virtual async Task<UserProject> GetUserProjectByProjectIdAndUSerId(int projectId, string userId)
        {
            return _context.UserProjects.FirstOrDefault(up => up.ProjectId == projectId && up.UserId == userId);
        }
    }
}
