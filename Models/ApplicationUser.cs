using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace FakeRomanHsieh.API.Models
{
    public class ApplicationUser: IdentityUser
    {
        public String Address { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
        public ICollection<Order> Orders { get; set; }
        public virtual ICollection<IdentityUserRole<String>> UserRoles { get; set; }
        //public virtual ICollection<IdentityUserClaim<String>> Claims { get; set; }
        //public virtual ICollection<IdentityUserLogin<String>> Logins { get; set; }
        //public virtual ICollection<IdentityUserToken<String>> Tokens { get; set; }
    }
}
