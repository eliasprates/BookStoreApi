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
    public class AuthorsControllerTests : IDisposable
    {
        private readonly AuthorsController _controller;
        private readonly BookStoreContext _context;

        public AuthorsControllerTests()
        {
            _context = DbContextHelper.GetInMemoryDbContext();
            _controller = new AuthorsController(_context);
        }

        [Fact]
        public async Task GetAuthors_ShouldReturnEmptyList_WhenNoAuthorsExist()
        {
            // Arrange
            _context.Authors.RemoveRange(_context.Authors); // Limpa os autores existentes no contexto
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetAuthors();

            // Assert
            Assert.IsType<ActionResult<IEnumerable<Author>>>(result);
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task GetAuthor_ShouldReturnAuthor_WhenAuthorExists()
        {
            // Arrange
            var author = new Author { Name = "George Orwell" };
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetAuthor(author.Id);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Author>>(result);
            var returnValue = Assert.IsType<Author>(actionResult.Value);
            Assert.Equal(author.Id, returnValue.Id);               // Verifica o ID
            Assert.Equal("George Orwell", returnValue.Name); // Verifica o Nome
        }

        [Fact]
        public async Task CreateAuthor_ShouldAddAuthorSuccessfully()
        {
            // Arrange
            var author = new Author { Name = "Aldous Huxley" };

            // Act
            var result = await _controller.PostAuthor(author);

            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<Author>(actionResult.Value);
            Assert.Equal("Aldous Huxley", returnValue.Name); // Verifica o Nome
        }

        [Fact]
        public async Task DeleteAuthor_ShouldRemoveAuthorSuccessfully()
        {
            // Arrange
            var author = new Author { Id = 1, Name = "George Orwell" };
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteAuthor(1);

            // Assert
            Assert.IsType<NoContentResult>(result);  // Verifica se a remoção foi bem-sucedida
            Assert.Null(await _context.Authors.FindAsync(1));  // Verifica se o autor foi removido
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted(); // Limpa o banco de dados após cada teste
            _context.Dispose();
        }
    }
}
