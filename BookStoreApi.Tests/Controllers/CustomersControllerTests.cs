using BookStoreApi.Controllers;
using BookStoreApi.Data;
using BookStoreApi.Models;
using BookStoreApi.Tests.TestHelpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreApi.Tests.Controllers
{
    public class CustomersControllerTests : IDisposable
    {
        private readonly CustomersController _controller;
        private readonly BookStoreContext _context;

        public CustomersControllerTests()
        {
            _context = DbContextHelper.GetInMemoryDbContext();
            _controller = new CustomersController(_context);
        }

        [Fact]
        public async Task GetCustomers_ShouldReturnEmptyList_WhenNoCustomersExist()
        {
            // Arrange
            _context.Customers.RemoveRange(_context.Customers); // Limpa os clientes existentes
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetCustomers();

            // Assert
            Assert.IsType<ActionResult<IEnumerable<Customer>>>(result);
            Assert.Empty(result.Value);  // Verifica se a lista está vazia
        }

        [Fact]
        public async Task GetCustomer_ShouldReturnCustomer_WhenCustomerExists()
        {
            // Arrange
            var customer = new Customer { Id = 1, Name = "John Doe", IsVip = true };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetCustomer(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Customer>>(result);
            var returnValue = Assert.IsType<Customer>(actionResult.Value);
            Assert.Equal(1, returnValue.Id);           // Verifica o ID
            Assert.Equal("John Doe", returnValue.Name); // Verifica o Nome
            Assert.True(returnValue.IsVip);            // Verifica o status de VIP
        }

        [Fact]
        public async Task CreateCustomer_ShouldAddCustomerSuccessfully()
        {
            // Arrange
            var customer = new Customer { Name = "Jane Doe", IsVip = false };

            // Act
            var result = await _controller.PostCustomer(customer);

            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<Customer>(actionResult.Value);
            Assert.Equal("Jane Doe", returnValue.Name); // Verifica o Nome
            Assert.False(returnValue.IsVip);           // Verifica o status de VIP
        }

        [Fact]
        public async Task DeleteCustomer_ShouldRemoveCustomerSuccessfully()
        {
            // Arrange
            var customer = new Customer { Name = "John Doe", IsVip = true };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteCustomer(customer.Id);

            // Assert
            Assert.IsType<NoContentResult>(result);  // Verifica se a remoção foi bem-sucedida
            Assert.Null(await _context.Customers.FindAsync(customer.Id));  // Verifica se o cliente foi removido
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted(); // Limpa o banco de dados após cada teste
            _context.Dispose();
        }
    }
}
