﻿namespace BookStoreApi.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsVip { get; set; }
    }
}
