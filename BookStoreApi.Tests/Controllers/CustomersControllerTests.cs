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

        [Theory]
        [InlineData(0, null, false)]  // Nenhum cliente existente
        [InlineData(1, "John Doe", true)]  // Cliente existente
        public async Task GetCustomersAndCustomer_ShouldReturnExpectedResults(int expectedCustomerCount, string customerName, bool isVip)
        {
            // Arrange
            if (expectedCustomerCount > 0)
            {
                var customer = new Customer { Id = 1, Name = customerName, IsVip = isVip };
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
            }

            // Act - Testando a listagem de clientes
            var resultList = await _controller.GetCustomers();

            // Assert - Verificando a lista de clientes
            Assert.IsType<ActionResult<IEnumerable<Customer>>>(resultList);
            if (expectedCustomerCount == 0)
            {
                Assert.Empty(resultList.Value); // Verifica se a lista está vazia
            }
            else
            {
                Assert.NotEmpty(resultList.Value);

                // Act - Testando o retorno de um cliente específico
                var result = await _controller.GetCustomer(1);

                // Assert - Verificando os detalhes do cliente retornado
                var actionResult = Assert.IsType<ActionResult<Customer>>(result);
                var returnValue = Assert.IsType<Customer>(actionResult.Value);
                Assert.Equal(1, returnValue.Id);  // Verifica o ID
                Assert.Equal(customerName, returnValue.Name);  // Verifica o Nome
                Assert.Equal(isVip, returnValue.IsVip);  // Verifica o status de VIP
            }
        }

        [Theory]
        [InlineData("Jane Doe", false)]  // Cliente não VIP
        [InlineData("John Doe", true)]   // Cliente VIP
        public async Task CreateCustomer_ShouldAddCustomerSuccessfully(string customerName, bool isVip)
        {
            // Arrange
            var customer = new Customer { Name = customerName, IsVip = isVip };

            // Act
            var result = await _controller.PostCustomer(customer);

            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<Customer>(actionResult.Value);
            Assert.Equal(customerName, returnValue.Name);  // Verifica o Nome
            Assert.Equal(isVip, returnValue.IsVip);        // Verifica o status de VIP
        }


        [Theory]
        [InlineData("John Doe", true)]  // Cliente VIP
        [InlineData("Jane Doe", false)]  // Cliente não VIP
        public async Task DeleteCustomer_ShouldRemoveCustomerSuccessfully(string customerName, bool isVip)
        {
            // Arrange
            var customer = new Customer { Name = customerName, IsVip = isVip };
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
