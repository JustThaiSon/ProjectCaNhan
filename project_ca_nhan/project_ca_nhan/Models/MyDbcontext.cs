using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using project_ca_nhan.Models;
namespace project_ca_nhan.Models
{
    public class MyDbcontext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();

            string strConnectionString = config.GetConnectionString("MyConnectionString");
            optionsBuilder.UseSqlServer(strConnectionString);
        }
        public DbSet<ItemUsers> Users { get; set; }
        public DbSet<ItemCategory> Categories { get; set; }
        public DbSet<ItemAdv> Adv { get; set; }
        public DbSet<ItemCustomer> Customers { get; set; }
        public DbSet<ItemNews> News { get; set; }
        public DbSet<ItemOrder> Orders { get; set; }
        public DbSet<ItemOrderDetail> OrdersDetail { get; set; }
        public DbSet<ItemProduct> Products { get; set; }
        public DbSet<ItemRating> Rating { get; set; }
        public DbSet<ItemSlide> Slides { get; set; }
        public DbSet<ItemTag> Tags { get; set; }
        public DbSet<ItemTagProduct> TagsProducts { get; set; }
        public DbSet<ItemCategoryProduct> CategoriesProducts { get; set; }
        public DbSet<ItemRespond> Respond { get; set; }
    }
}
