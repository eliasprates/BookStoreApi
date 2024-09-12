using System.ComponentModel.DataAnnotations;

namespace BookStoreApi.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsVip { get; set; }
    }
}
