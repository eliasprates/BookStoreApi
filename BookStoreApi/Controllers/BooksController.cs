﻿using BookStoreApi.Data;
using BookStoreApi.Models;
using BookStoreApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookStoreContext _context;
        private readonly IBookService _bookService;

        public BooksController(BookStoreContext context, IBookService bookService)
        {
            _context = context;
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            return await _context.Books.Include(b => b.Author).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _context.Books.Include(b => b.Author).FirstOrDefaultAsync(b => b.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            return book;
        }

        [HttpPost("purchase")]
        public async Task<IActionResult> PurchaseBook(int bookId, int customerId, int quantity)
        {
            var book = await _context.Books.FindAsync(bookId);
            var customer = await _context.Customers.FindAsync(customerId);

            if (book == null || customer == null)
            {
                return NotFound("Book or customer not found.");
            }

            if (!_bookService.IsBookInStock(book, quantity))
            {
                return BadRequest("Not enough stock.");
            }

            var finalPrice = _bookService.CalculateDiscountedPrice(book, customer) * quantity;
            return Ok(new { finalPrice });
        }
    }
}
