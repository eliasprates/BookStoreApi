using BookStoreApi.Data;
using BookStoreApi.Tests.TestHelpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreApi.Tests.Data
{
    public class BookStoreContextTests : IDisposable
    {
        private readonly BookStoreContext _context;

        public BookStoreContextTests()
        {
            // Inicializa o contexto de banco de dados em memória usando DbContextHelper
            _context = DbContextHelper.GetInMemoryDbContext();

            // Certifica-se de que o banco de dados em memória está criado e os dados seed aplicados
            _context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();  // Apaga o banco de dados após cada teste
            _context.Dispose();                 // Libera os recursos utilizados pelo contexto
        }

       [Fact]
        public void Author_ShouldHaveManyBooks()
        {
            // Act
            var author = _context.Authors.Include(a => a.Books).FirstOrDefault(a => a.Id == 1);

            // Assert
            Assert.NotNull(author);  // Garante que o autor não é nulo
            Assert.NotNull(author.Books);  // Garante que o autor tem livros
            Assert.True(author.Books.Count > 0);  // Verifica se o autor tem pelo menos um livro
        }

        /*[Fact]
        public void ShouldContainSeedData()
        {
            // Act
            var author = _context.Authors.Find(1);
            var book = _context.Books.Find(1);
            var customerVip = _context.Customers.Find(1);
            var customerNonVip = _context.Customers.Find(2);

            // Assert
            Assert.NotNull(author);
            Assert.Equal("George Orwell", author.Name);

            Assert.NotNull(book);
            Assert.Equal("1984", book.Title);

            Assert.NotNull(customerVip);
            Assert.True(customerVip.IsVip);

            Assert.NotNull(customerNonVip);
            Assert.False(customerNonVip.IsVip);
        }*/
        
    }
}
