using JelloTicket.DataLayer.Data;
using JelloTicket.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JelloTicket.DataLayer.Repositories
{
    public class ProjectRepo : IRepository<Project>
    {
        private ApplicationDbContext _context;

        public ProjectRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public Project Get(int? id)
        {
            return (Project)_context.Projects.FirstOrDefault(t => t.Id == id);
        }

        public ICollection<Project> GetAll()
        {
            return _context.Projects.ToHashSet();
        }

        public void Create(Project project)
        {
            _context.Add(project);
            _context.SaveChanges();
        }

        public void Update(Project project)
        {
            _context.Update(project);
            _context.SaveChanges();
        }

        public void Delete(int? id)
        {
            _context.Projects.Remove(_context.Projects.First(t => t.Id == id));
            _context.SaveChanges();
        }
    }
}
