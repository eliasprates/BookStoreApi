using BookStoreApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreApi.Tests.Models
{
    public class BookTests
    {
        [Fact]
        public void Book_ShouldInitializeWithCorrectProperties()
        {
            // Arrange & Act
            var book = new Book { Id = 1, Title = "1984", Price = 19.99m, StockQuantity = 10 };

            // Assert
            Assert.Equal(1, book.Id);                   // Verifica se o ID foi definido corretamente
            Assert.Equal("1984", book.Title);           // Verifica se o Título foi definido corretamente
            Assert.Equal(19.99m, book.Price);           // Verifica se o Preço foi definido corretamente
            Assert.Equal(10, book.StockQuantity);       // Verifica se a Quantidade no Estoque foi definida corretamente
        }

        [Fact]
        public void Book_ShouldAllowUpdatingStockQuantity()
        {
            // Arrange
            var book = new Book { StockQuantity = 10 };

            // Act
            book.StockQuantity -= 2;

            // Assert
            Assert.Equal(8, book.StockQuantity);  // Verifica se a quantidade no estoque foi atualizada corretamente
        }
    }
}
