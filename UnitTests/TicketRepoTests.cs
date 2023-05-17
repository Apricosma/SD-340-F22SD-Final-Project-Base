using JelloTicket.DataLayer.Data;
using JelloTicket.DataLayer.Models;
using JelloTicket.DataLayer.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using JelloTicket.DataLayer.Repositories;
using NuGet.ContentModel;
using Moq;

namespace UnitTests
{
    [TestClass]
    public class TicketRepoTests
    {
        [TestMethod]
        public void Get_Id_ReturnsTicket() 
        {
            var mockRepo = new Mock<IRepository<Ticket>>();
            var repository = mockRepo.Object;

            // Act
            Ticket result = repository.Get(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);

        }

    }
}