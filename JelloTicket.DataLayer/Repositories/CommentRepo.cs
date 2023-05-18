using JelloTicket.DataLayer.Data;
using JelloTicket.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JelloTicket.DataLayer.Repositories
{
    public class CommentRepo : IRepository<Comment>
    {
        private ApplicationDbContext _context;

        public CommentRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public Comment Get(int? id)
        {
            return (Comment)_context.Comments.Where(t => t.Id == id);
        }

        public ICollection<Comment> GetAll()
        {
            return _context.Comments.ToHashSet();
        }

        public void Create(Comment comment)
        {
            _context.Add(comment);
            _context.SaveChanges();
        }

        public void Update(Comment comment)
        {
            _context.Update(comment);
            _context.SaveChanges();
        }

        public void Delete(int? id)
        {
            _context.Comments.Remove(_context.Comments.First(t => t.Id == id));
            _context.SaveChanges();
        }
    }
}
