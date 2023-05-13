﻿using JelloTicket.DataLayer.Data;
using JelloTicket.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JelloTicket.DataLayer.Repositories
{
    public class TicketRepo : IRepository<Ticket>
    {
        private ApplicationDbContext _context;

        public TicketRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public Ticket Get(int? id)
        {
            return (Ticket)_context.Tickets.Where(t => t.Id == id);
        }

        public ICollection<Ticket> GetAll()
        {
            return _context.Tickets.ToHashSet();
        }

        public void Create(Ticket ticket)
        {
            _context.Add(ticket);
            _context.SaveChanges();
        }

        public void Update(Ticket ticket)
        {
            _context.Update(ticket);
            _context.SaveChanges();
        }

        public void Delete(int? id)
        {
            _context.Tickets.Remove(_context.Tickets.First(t => t.Id == id));
            _context.SaveChanges();
        }
    }
}
