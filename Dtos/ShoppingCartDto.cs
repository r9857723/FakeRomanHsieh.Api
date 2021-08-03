using System;
using System.Collections.Generic;

namespace FakeRomanHsieh.API.Dtos
{
    public class ShoppingCartDto
    {
        public Guid Id { get; set; }
        public String UserId { get; set; }
        public ICollection<LineItemDto> ShoppingCartItems { get; set; }
    }
}
