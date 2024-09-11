using BookStoreApi.Models;

namespace BookStoreApi.Services
{
    public class BookService : IBookService
    {
        public decimal CalculateDiscountedPrice(Book book, Customer customer)
        {
            if (customer.IsVip)
            {
                return book.Price * 0.9m; // 10% de desconto
            }
            return book.Price;
        }

        public bool IsBookInStock(Book book, int quantityRequested)
        {
            return book.StockQuantity >= quantityRequested;
        }
    }
}
