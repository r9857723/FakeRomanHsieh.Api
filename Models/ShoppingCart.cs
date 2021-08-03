using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FakeRomanHsieh.API.Models
{
    public class ShoppingCart
    {
        [Key]
        public Guid Id { get; set; }
        public String UserId { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<LineItem> ShoppingCartItems { get; set; }
    }
}
