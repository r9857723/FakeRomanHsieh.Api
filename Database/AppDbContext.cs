using FakeRomanHsieh.API.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace FakeRomanHsieh.API.Database
{
    public class AppDbContext: IdentityDbContext<ApplicationUser> //DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options) 
        {

        }

        public DbSet<TouristRoute> TouristRoutes { get; set; }
        public DbSet<TouristRoutePicture> TouristRoutePictures { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<LineItem> LineItems { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var touristRouteJsonData = File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"/Database/touristRoutesMockData.json");
            IList<TouristRoute> touristRoutes = JsonConvert.DeserializeObject<IList<TouristRoute>>(touristRouteJsonData);
            modelBuilder.Entity<TouristRoute>().HasData(touristRoutes);

            var touristRoutePictureJsonData = File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"/Database/touristRoutePicturesMockData.json");
            IList<TouristRoutePicture> touristRoutePictures = JsonConvert.DeserializeObject<IList<TouristRoutePicture>>(touristRoutePictureJsonData);
            modelBuilder.Entity<TouristRoutePicture>().HasData(touristRoutePictures);

            // 初始化用戶與角色的種子數據
            // 1.更新用戶及角色的外鍵關係
            modelBuilder.Entity<ApplicationUser>(user => 
                user.HasMany(x => x.UserRoles).WithOne().HasForeignKey(userRole => userRole.UserId).IsRequired()
            );

            // 2.添加管理員角色
            var adminRoleId = "FB6D4F10-79ED-4AFF-A915-4CE29DC9C7E9";
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole()
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "Admin".ToUpper()
                }
            ) ;

            // 3.添加用戶
            var adminUserId = "XXXD4F10-79ED-4AFF-A915-4CE29DC9C7E9";
            ApplicationUser applicationUser = new ApplicationUser()
            {
                Id = adminUserId,
                UserName = "admin@FakeRomanHsieh.com",
                NormalizedUserName = "admin@FakeRomanHsieh.com".ToUpper(),
                Email = "admin@FakeRomanHsieh.com",
                NormalizedEmail = "admin@FakeRomanHsieh.com".ToUpper(),
                TwoFactorEnabled = false,
                EmailConfirmed = true,
                PhoneNumber = "0912456789",
                PhoneNumberConfirmed = false
            };
            var ph = new PasswordHasher<ApplicationUser>();
            applicationUser.PasswordHash = ph.HashPassword(applicationUser, "Roman123$");
            modelBuilder.Entity<ApplicationUser>().HasData(applicationUser);

            // 4.給用戶加入管理員角色
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>()
                {
                    RoleId = adminRoleId,
                    UserId = adminUserId
                }
            );

            base.OnModelCreating(modelBuilder);

        }
    }
}
