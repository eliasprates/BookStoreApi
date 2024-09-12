using BookStoreApi.Controllers;
using BookStoreApi.Data;
using BookStoreApi.Dtos;
using BookStoreApi.Models;
using BookStoreApi.Services;
using BookStoreApi.Tests.TestHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        [Fact]
        public async Task GetBooks_ShouldReturnEmptyList_WhenNoBooksExist()
        {
            // Act
            var result = await _controller.GetBooks();

            // Assert
            Assert.IsType<ActionResult<IEnumerable<Book>>>(result);
            Assert.Empty(result.Value); // Verifica se a lista está vazia
        }

        [Fact]
        public async Task GetBook_ShouldReturnBook_WhenBookExists()
        {
            // Arrange
            var author = new Author { Id = 1, Name = "George Orwell" };  // Criando o autor para evitar falha de relacionamento
            var book = new Book { Id = 1, Title = "1984", Price = 19.99m, StockQuantity = 10, Author = author};
            _context.Authors.Add(author);  // Adicionando o autor ao contexto
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetBook(book.Id); // Usando o ID correto que foi salvo no banco

            // Assert
            var actionResult = Assert.IsType<ActionResult<Book>>(result);
            var returnValue = Assert.IsType<Book>(actionResult.Value);
            Assert.Equal(book.Id, returnValue.Id);
            Assert.Equal("1984", returnValue.Title);
            Assert.Equal(19.99m, returnValue.Price);
            Assert.Equal(10, returnValue.StockQuantity);
            Assert.Equal("George Orwell", returnValue.Author.Name); // Verificando o nome do autor
        }

        [Fact]
        public async Task PurchaseBook_ShouldReturnDiscountedPrice_ForVipCustomer()
        {
            // Arrange
            var book = new Book { Id = 1, Title = "1984", Price = 100m, StockQuantity = 10 };
            var vipCustomer = new Customer { Id = 1, Name = "John Doe", IsVip = true };
            _context.Books.Add(book);
            _context.Customers.Add(vipCustomer);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.PurchaseBook(1, 1, 1);

            // Verifique se o tipo de retorno está correto
            var actionResult = Assert.IsType<OkObjectResult>(result);

            // Verifique o valor de retorno (já tipado com DTO)
            var returnValue = Assert.IsType<FinalPriceDto>(actionResult.Value);
            Assert.Equal(90m, returnValue.FinalPrice);  // Verifica o valor de finalPrice
        }

        [Fact]
        public async Task PurchaseBook_ShouldReturnFullPrice_ForNonVipCustomer()
        {
            // Arrange
            var book = new Book { Id = 1, Title = "1984", Price = 100m, StockQuantity = 10 };
            var customer = new Customer { Id = 1, Name = "Jane Doe", IsVip = false }; // Cliente não VIP
            _context.Books.Add(book);
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.PurchaseBook(1, 1, 1);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<FinalPriceDto>(actionResult.Value);
            Assert.Equal(100m, returnValue.FinalPrice);  // Verifica se o preço cheio foi cobrado
        }

        [Fact]
        public async Task PurchaseBook_ShouldReturnNotFound_WhenBookIsNull()
        {
            // Arrange
            var vipCustomer = new Customer { Id = 1, Name = "John Doe", IsVip = true };
            _context.Customers.Add(vipCustomer);
           await _context.SaveChangesAsync();

            // Act
            var result = await _controller.PurchaseBook(999, 1, 1);  // Livro com ID inexistente

            // Assert
            var actionResult = Assert.IsType<NotFoundObjectResult>(result);  // Verifica se o retorno é NotFound
            var returnValue = actionResult.Value;  // Verifica que há uma mensagem
            Assert.NotNull(returnValue);  // A mensagem existe, mas não validamos o conteúdo exato
        }

        [Fact]
        public async Task PurchaseBook_ShouldReturnNotFound_WhenCustomerIsNull()
        {
            // Arrange
            var book = new Book { Id = 1, Title = "1984", Price = 100m, StockQuantity = 10 };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.PurchaseBook(1, 999, 1);  // Cliente com ID inexistente

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);  // Verifica se o retorno é NotFound
        }

        [Fact]
        public async Task PurchaseBook_ShouldReturnBadRequest_WhenStockIsNotSufficient()
        {
            // Arrange
            var book = new Book { Id = 1, Title = "1984", Price = 100m, StockQuantity = 5 };  // Use um ID diferente
            var customer = new Customer { Id = 1, Name = "John Doe", IsVip = false };          // Use um ID diferente
            _context.Books.Add(book);
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.PurchaseBook(1, 1, 10); // Correspondência de IDs

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Not enough stock.", actionResult.Value);  // Verifica a mensagem de erro
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted(); // Limpa o banco de dados após cada teste
            _context.Dispose();
        }

    }
}
