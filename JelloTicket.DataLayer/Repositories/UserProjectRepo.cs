using JelloTicket.DataLayer.Data;
using JelloTicket.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JelloTicket.DataLayer.Repositories
{
    public class UserProjectRepo : IRepository<UserProject>
    {
        private ApplicationDbContext _context;

        public UserProjectRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public UserProject Get(int? id)
        {
            return (UserProject)_context.UserProjects.FirstOrDefault(t => t.ProjectId == id);
        }

        public ICollection<UserProject> GetAll()
        {
            return _context.UserProjects.ToHashSet();
        }

        public void Create(UserProject up)
        {
            _context.Add(up);
            _context.SaveChanges();
        }

        public void Update(UserProject up)
        {
            _context.Update(up);
            _context.SaveChanges();
        }

        public void Delete(int? id)
        {
            _context.UserProjects.Remove(_context.UserProjects.First(t => t.Id == id));
            _context.SaveChanges();
        }
    }
}
