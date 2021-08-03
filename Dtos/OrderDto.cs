using System;
using System.Collections.Generic;
using FakeRomanHsieh.API.Dtos;
using FakeRomanHsieh.API.Models;

namespace FakeRomanHsieh.API.Helper
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public String UserId { get; set; }
        public ICollection<LineItemDto> OrderItems { get; set; }
        public String State { get; set; }
        public DateTime CreateDateUtc { get; set; }
        public String TranscationMetadata { get; set; }
    }
}
