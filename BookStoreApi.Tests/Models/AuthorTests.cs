using BookStoreApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreApi.Tests.Models
{
    public class AuthorTests
    {
        [Fact]
        public void Author_ShouldInitializeWithEmptyBooksList()
        {
            // Arrange & Act
            var author = new Author();

            // Assert
            Assert.NotNull(author.Books); // Verifica se a lista de livros não é nula
            Assert.Empty(author.Books);   // Verifica se a lista começa vazia
        }

        [Fact]
        public void Author_ShouldAllowSettingProperties()
        {
            // Arrange
            var author = new Author();

            // Act
            author.Id = 1;
            author.Name = "George Orwell";
            var book = new Book { Id = 1, Title = "1984" };
            author.Books.Add(book);

            // Assert
            Assert.Equal(1, author.Id);
            Assert.Equal("George Orwell", author.Name);
            Assert.Single(author.Books);
            Assert.Equal("1984", author.Books[0].Title);
        }

        [Fact]
        public void Author_ShouldAllowRemovingBooks()
        {
            // Arrange
            var author = new Author();
            var book = new Book { Id = 1, Title = "1984" };
            author.Books.Add(book);

            // Act
            author.Books.Remove(book);

            // Assert
            Assert.Empty(author.Books);  // Verifica se o livro foi removido corretamente
        }
    }
}
