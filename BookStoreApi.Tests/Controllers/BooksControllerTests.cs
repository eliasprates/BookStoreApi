using BookStoreApi.Controllers;
using BookStoreApi.Data;
using BookStoreApi.Dtos;
using BookStoreApi.Models;
using BookStoreApi.Services;
using BookStoreApi.Tests.TestHelpers;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreApi.Tests.Controllers
{
    public class BooksControllerTests : IDisposable
    {
        private readonly BooksController _controller;
        private readonly BookStoreContext _context;

        public BooksControllerTests()
        {
            _context = DbContextHelper.GetInMemoryDbContext();
            _controller = new BooksController(_context, new BookService());
        }

        [Theory]
        [InlineData(0, null, null, 0, 0, null)]
        [InlineData(1, "1984", "George Orwell", 19.99, 10, 1)]
        public async Task GetBooksAndGetBook_ShouldReturnExpectedResults(
        int expectedBookCount, string title, string authorName, decimal price, 
        int stockQuantity, int? bookId)
        {
            // Arrange
            if (expectedBookCount > 0)
            {
                var author = new Author { Id = 1, Name = authorName };
                var book = new Book { Id = bookId.Value, Title = title, Price = price, StockQuantity = stockQuantity, Author = author };
                _context.Authors.Add(author);
                _context.Books.Add(book);
                await _context.SaveChangesAsync();
            }

            // Act
            var resultList = await _controller.GetBooks();

            // Assert
            Assert.IsType<ActionResult<IEnumerable<Book>>>(resultList);
            if (expectedBookCount == 0)
            {
                Assert.Empty(resultList.Value); 
            }
            else
            {
                Assert.NotEmpty(resultList.Value);

                // Act
                var result = await _controller.GetBook(bookId.Value);

                // Assert
                var actionResult = Assert.IsType<ActionResult<Book>>(result);
                var returnValue = Assert.IsType<Book>(actionResult.Value);
                Assert.Equal(bookId, returnValue.Id);
                Assert.Equal(title, returnValue.Title);
                Assert.Equal(price, returnValue.Price);
                Assert.Equal(stockQuantity, returnValue.StockQuantity);
                Assert.Equal(authorName, returnValue.Author.Name); 
            }
        }

        [Theory]
        [InlineData(999, 1)]
        [InlineData(1, 999)]
        public async Task PurchaseBook_ShouldReturnNotFound_WhenBookOrCustomerDoesNotExist(int bookId, int customerId)
        {
            // Arrange
            var book = new Book { Id = 1, Title = "1984", Price = 100m, StockQuantity = 10 };
            var customer = new Customer { Id = 1, Name = "John Doe", IsVip = false };

            if (bookId != 999)
                _context.Books.Add(book);
            if (customerId != 999)
                _context.Customers.Add(customer);

            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.PurchaseBook(bookId, customerId, 1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Theory]
        [InlineData(5, 10, "Not enough stock.")]
        [InlineData(0, 1, "Not enough stock.")]
        [InlineData(-1, 1, "Not enough stock.")]
        public async Task PurchaseBook_ShouldReturnBadRequest_ForInvalidStock(int stockQuantity, int purchaseQuantity, string expectedErrorMessage)
        {
            // Arrange
            var book = new Book { Id = 1, Title = "1984", Price = 100m, StockQuantity = stockQuantity };
            var customer = new Customer { Id = 1, Name = "John Doe", IsVip = false };
            _context.Books.Add(book);
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.PurchaseBook(1, 1, purchaseQuantity);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(expectedErrorMessage, actionResult.Value);
        }

        [Theory]
        [InlineData(1, "John Doe", true, 90.0)]
        [InlineData(2, "Jane Doe", false, 100.0)]
        public async Task PurchaseBook_ShouldReturnCorrectPrice(int customerId, string customerName, bool isVip, decimal expectedPrice)
        {
            // Arrange
            var book = new Book { Id = 1, Title = "1984", Price = 100m, StockQuantity = 10 };
            var customer = new Customer { Id = customerId, Name = customerName, IsVip = isVip };
            _context.Books.Add(book);
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.PurchaseBook(1, customerId, 1);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<FinalPriceDto>(actionResult.Value);
            Assert.Equal(expectedPrice, returnValue.FinalPrice);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

    }
}
