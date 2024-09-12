using BookStoreApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreApi.Tests.Models
{
    public class CustomerTests
    {
        [Fact]
        public void Customer_ShouldInitializeWithCorrectProperties()
        {
            // Arrange & Act
            var customer = new Customer { Id = 1, Name = "John Doe", IsVip = true };

            // Assert
            Assert.Equal(1, customer.Id);             // Verifica se o ID foi definido corretamente
            Assert.Equal("John Doe", customer.Name);  // Verifica se o Nome foi definido corretamente
            Assert.True(customer.IsVip);              // Verifica se a condição de VIP foi definida corretamente
        }

        [Fact]
        public void Customer_ShouldAllowUpdatingVipStatus()
        {
            // Arrange
            var customer = new Customer { IsVip = false };

            // Act
            customer.IsVip = true;

            // Assert
            Assert.True(customer.IsVip);  // Verifica se o status de VIP foi atualizado corretamente
        }
    }
}
