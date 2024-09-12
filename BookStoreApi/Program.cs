using BookStoreApi.Data;
using BookStoreApi.Models;
using BookStoreApi.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

/*Enables support for circular references, 
  preserving referenced objects and ensuring that 
  loops between related objects are handled correctly.*/
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });

builder.Services.AddDbContext<BookStoreContext>(options =>
    options.UseInMemoryDatabase("BookstoreDb"));
builder.Services.AddScoped<IBookService, BookService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

// **Adicionando seed data para o banco de dados em memória**
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BookStoreContext>();
    dbContext.Database.EnsureCreated();

    // Adicionando dados de seed se não existirem
    /*if (!dbContext.Authors.Any())
    {
        dbContext.Authors.Add(new Author { Id = 3, Name = "George Orwell" });
        dbContext.Books.Add(new Book { Id = 3, Title = "1984", Price = 100m, StockQuantity = 10, AuthorId = 1 });
        dbContext.Customers.Add(new Customer { Id = 3, Name = "John Doe", IsVip = true });
        await dbContext.SaveChangesAsync();
    }*/
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
