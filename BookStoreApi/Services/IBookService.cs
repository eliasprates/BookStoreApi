using BookStoreApi.Models;

namespace BookStoreApi.Services
{
    public interface IBookService
    {
        decimal CalculateDiscountedPrice(Book book, Customer customer);
        bool IsBookInStock(Book book, int quantityRequested);
    }
}
