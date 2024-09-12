using BookStoreApi.Models;
using BookStoreApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreApi.Tests.Services
{
    public class BookServiceTests
    {
        private readonly IBookService _bookService;

        public BookServiceTests()
        {
            _bookService = new BookService(); // Sem implementação ainda
        }

        [Fact]
        public void CalculateDiscountedPrice_ShouldApply10PercentDiscount_ForVipCustomer()
        {
            // Arrange
            var book = new Book { Price = 100m };
            var vipCustomer = new Customer { IsVip = true };

            // Act
            var discountedPrice = _bookService.CalculateDiscountedPrice(book, vipCustomer);

            // Assert
            Assert.Equal(90m, discountedPrice); // 10% de desconto para VIP
        }

        [Fact]
        public void CalculateDiscountedPrice_ShouldNotApplyDiscount_ForNonVipCustomer()
        {
            // Arrange
            var book = new Book { Price = 100m };
            var regularCustomer = new Customer { IsVip = false };

            // Act
            var discountedPrice = _bookService.CalculateDiscountedPrice(book, regularCustomer);

            // Assert
            Assert.Equal(100m, discountedPrice); // Sem desconto para cliente não VIP
        }

        [Fact]
        public void IsBookInStock_ShouldReturnTrue_WhenRequestedQuantityIsAvailable()
        {
            // Arrange
            var book = new Book { StockQuantity = 10 };
            var requestedQuantity = 5;

            // Act
            var isAvailable = _bookService.IsBookInStock(book, requestedQuantity);

            // Assert
            Assert.True(isAvailable);
        }

        [Fact]
        public void IsBookInStock_ShouldReturnFalse_WhenRequestedQuantityIsNotAvailable()
        {
            // Arrange
            var book = new Book { StockQuantity = 10 };
            var requestedQuantity = 15;

            // Act
            var isAvailable = _bookService.IsBookInStock(book, requestedQuantity);

            // Assert
            Assert.False(isAvailable);
        }

        [Fact]
        public void IsBookInStock_ShouldReturnTrue_WhenRequestedQuantityIsLessThanOrEqualToStock()
        {
            // Arrange
            var book = new Book { StockQuantity = 10 };

            // Act & Assert
            // Quando a quantidade solicitada é menor que o estoque
            Assert.True(_bookService.IsBookInStock(book, 5));

            // Quando a quantidade solicitada é exatamente igual ao estoque
            Assert.True(_bookService.IsBookInStock(book, 10));
        }

        [Fact]
        public void IsBookInStock_ShouldReturnFalse_WhenRequestedQuantityIsGreaterThanStock()
        {
            // Arrange
            var book = new Book { StockQuantity = 10 };

            // Act & Assert
            // Quando a quantidade solicitada é maior que o estoque
            Assert.False(_bookService.IsBookInStock(book, 15));
        }
    }
}
