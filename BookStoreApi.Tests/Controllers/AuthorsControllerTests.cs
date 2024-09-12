using BookStoreApi.Controllers;
using BookStoreApi.Data;
using BookStoreApi.Models;
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
    public class AuthorsControllerTests
    {
        [Theory]
        [InlineData(0, typeof(NotFoundResult))]
        [InlineData(1, typeof(OkObjectResult))]
        public async Task GetAuthorById_ReturnsExpectedResult(int authorId, Type expectedActionResultType)
        {
            // Arrange
            var context = DbContextHelper.GetInMemoryDbContext();

            if (authorId != 0)
            {
                // Certifique-se de que o autor é adicionado com o ID correto
                var author = new Author { Id = authorId, Name = "Test Author" };
                context.Authors.Add(author);
                await context.SaveChangesAsync(); // Certifique-se de salvar as mudanças no banco de dados
            }

            var controller = new AuthorsController(context);

            // Act
            var result = await controller.GetAuthor(authorId);

            // Assert
            Assert.NotNull(result); // Verifique se o resultado não é nulo
            Assert.IsType(expectedActionResultType, result.Result);
        }

        [Theory]
        [InlineData("New Author", 201)]
        [InlineData("", 400)]   // Testando nome vazio
        [InlineData(null, 400)] // Testando nome nulo
        public async Task PostAuthor_ReturnsExpectedStatusCode(string authorName, int expectedStatusCode)
        {
            // Arrange
            var context = DbContextHelper.GetInMemoryDbContext();
            var controller = new AuthorsController(context);
            var author = new Author { Name = authorName };

            // Act
            var result = await controller.PostAuthor(author);

            // Assert
            if (expectedStatusCode == 201)
            {
                var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
                var addedAuthor = Assert.IsType<Author>(createdAtActionResult.Value);
                Assert.Equal(authorName, addedAuthor.Name);
                Assert.NotEqual(0, addedAuthor.Id);
            }
            else
            {
                Assert.IsType<BadRequestResult>(result.Result);  // Validação para BadRequest quando o nome é inválido (nulo ou vazio)
            }
        }

        /*[Theory]
        [InlineData(1, 204)]
        [InlineData(0, 404)]
        public async Task DeleteAuthor_ReturnsExpectedStatusCode(int authorId, int expectedStatusCode)
        {
            // Arrange
            var context = DbContextHelper.GetInMemoryDbContext();

            if (authorId != 0)
            {
                context.Authors.Add(new Author { Id = authorId, Name = "Author to delete" });
                await context.SaveChangesAsync();
            }

            var controller = new AuthorsController(context);

            // Act
            var result = await controller.DeleteAuthor(authorId);

            // Assert
            if (expectedStatusCode == 204)
            {
                Assert.IsType<NoContentResult>(result);
            }
            else
            {
                Assert.IsType<NotFoundResult>(result);
            }
        }*/

        [Theory]
        [InlineData(1, 204)]
        [InlineData(0, 404)]
        public async Task DeleteAuthor_ReturnsExpectedStatusCode(int authorId, int expectedStatusCode)
        {
            // Arrange
            var context = DbContextHelper.GetInMemoryDbContext();

            if (authorId != 0)
            {
                context.Authors.Add(new Author { Id = authorId, Name = "Author to delete" });
                await context.SaveChangesAsync();
            }

            var controller = new AuthorsController(context);

            // Act
            var result = await controller.DeleteAuthor(authorId);

            // Assert
            if (expectedStatusCode == 204)
            {
                Assert.IsType<NoContentResult>(result);
            }
            else
            {
                Assert.IsType<NotFoundResult>(result);
            }
        }

        [Theory]
        [InlineData(0)]  // Autor não encontrado
        [InlineData(999)]  // ID inválido ou fora do intervalo esperado
        public async Task GetAuthor_ShouldReturnNotFound_WhenAuthorDoesNotExist(int invalidId)
        {
            // Arrange
            var context = DbContextHelper.GetInMemoryDbContext();
            var controller = new AuthorsController(context);

            // Act
            var result = await controller.GetAuthor(invalidId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);  // Verifica se retorna NotFound
        }

        [Theory]
        [InlineData(1, 2)]  // Autor 1 com 2 livros
        [InlineData(2, 1)]  // Autor 2 com 1 livro
        public async Task GetAuthors_ShouldReturnAuthorsWithBooks(int authorId, int expectedBookCount)
        {
            // Arrange
            var context = DbContextHelper.GetInMemoryDbContext();

            var author1 = new Author { Id = 1, Name = "Author 1" };
            var author2 = new Author { Id = 2, Name = "Author 2" };

            // Adicionando livros associados aos autores
            var book1 = new Book { Id = 1, Title = "Book 1", Author = author1 };
            var book2 = new Book { Id = 2, Title = "Book 2", Author = author1 };
            var book3 = new Book { Id = 3, Title = "Book 3", Author = author2 };

            context.Authors.AddRange(author1, author2);
            context.Books.AddRange(book1, book2, book3);

            await context.SaveChangesAsync();

            var controller = new AuthorsController(context);

            // Act
            var result = await controller.GetAuthors();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Author>>>(result);
            var authors = Assert.IsType<List<Author>>(actionResult.Value);

            // Verifica se o autor específico foi retornado
            var authorResult = authors.First(a => a.Id == authorId);

            // Verifica se o número de livros do autor corresponde ao esperado
            Assert.Equal(expectedBookCount, authorResult.Books.Count);
        }

        [Theory]
        [InlineData("New Author", 201)]  // Autor válido
        [InlineData("", 400)]            // Nome do autor inválido (vazio)
        [InlineData(null, 400)]          // Nome do autor inválido (nulo)
        public async Task PostAuthor_ShouldReturnExpectedStatusCodeAndPersistData(string authorName, int expectedStatusCode)
        {
            // Arrange
            var context = DbContextHelper.GetInMemoryDbContext();
            var controller = new AuthorsController(context);
            var author = new Author { Name = authorName };

            // Act
            var result = await controller.PostAuthor(author);

            // Assert
            if (expectedStatusCode == 201)
            {
                // Verifica se o resultado é o esperado (201 Created)
                var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
                var addedAuthor = Assert.IsType<Author>(createdAtActionResult.Value);

                // Verifica se o autor foi adicionado com o nome correto e o ID foi gerado
                Assert.Equal(authorName, addedAuthor.Name);
                Assert.NotEqual(0, addedAuthor.Id);

                // Verifica se o autor foi realmente persistido no banco de dados
                var authorInDb = await context.Authors.FindAsync(addedAuthor.Id);
                Assert.NotNull(authorInDb);
                Assert.Equal(authorName, authorInDb.Name);  // Confirma que o autor foi salvo corretamente
            }
            else
            {
                // Verifica se o resultado é BadRequest para nomes inválidos
                Assert.IsType<BadRequestResult>(result.Result);
            }
        }

        [Theory]
        [InlineData(1, 1, "Updated Author", 204)]  // IDs correspondem, sucesso
        [InlineData(1, 2, "Updated Author", 400)]  // IDs não correspondem, BadRequest
        public async Task PutAuthor_ShouldReturnExpectedStatusCodeAndPersistData(int authorId, int passedId, string updatedName, int expectedStatusCode)
        {
            // Arrange
            var context = DbContextHelper.GetInMemoryDbContext();

            // Adiciona um autor ao contexto
            var existingAuthor = new Author { Id = authorId, Name = "Original Author" };
            context.Authors.Add(existingAuthor);
            await context.SaveChangesAsync();

            // Atualiza o autor com novos dados
            var updatedAuthor = await context.Authors.FindAsync(authorId);  // Buscando a instância já rastreada
            updatedAuthor.Name = updatedName;  // Atualizando o nome do autor

            var controller = new AuthorsController(context);

            // Act
            var result = await controller.PutAuthor(passedId, updatedAuthor);

            // Assert
            if (expectedStatusCode == 204)  // NoContent (sucesso na atualização)
            {
                Assert.IsType<NoContentResult>(result);

                // Verifica se o autor foi realmente atualizado no banco de dados
                var authorInDb = await context.Authors.FindAsync(authorId);
                Assert.NotNull(authorInDb);
                Assert.Equal(updatedName, authorInDb.Name);  // Verifica se o nome foi atualizado
            }
            else  // BadRequest quando os IDs não correspondem
            {
                Assert.IsType<BadRequestResult>(result);
            }
        }


    }
}
