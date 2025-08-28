using HotPot23API.Models;
using Microsoft.EntityFrameworkCore;

namespace HotPot23API.Contexts
{
    public class FoodDeliveryManagementContext:DbContext
    {
        public FoodDeliveryManagementContext(DbContextOptions options) : base(options) { }

        #region DbSet
        public DbSet<UserMaster> UserMasters { get; set; }
        public DbSet<UserAddressMaster> UserAddressMasters { get; set; }
        public DbSet<RestaurantMaster> RestaurantMasters { get; set; }
        public DbSet<CategoryMaster> CategoryMasters { get; set; }  // Changed here
        public DbSet<MenuItemMaster> MenuItems { get; set; }
        public DbSet<DiscountMaster> DiscountMasters { get; set; }
        public DbSet<CartTransaction> CartTransactions { get; set; }
        public DbSet<OrderTransaction> OrderTransactions { get; set; }
        public DbSet<OrderItemDetails> OrderItemDetails { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<DeliveryStatusTransaction> DeliveryStatusTransactions { get; set; }
        public DbSet<ReviewTransaction> ReviewTransactions { get; set; }
        public DbSet<User> Users { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // UserMaster
            modelBuilder.Entity<UserMaster>().HasKey(u => u.UserID);

            // User
            modelBuilder.Entity<User>().HasKey(u => u.Username);
            modelBuilder.Entity<User>()
                .HasOne(u => u.UserMaster)
                .WithOne(um => um.AuthUser)
                .HasForeignKey<User>(u => u.UserID)
                .HasConstraintName("FK_User_UserMaster")
                .OnDelete(DeleteBehavior.Restrict);

            // UserAddressMaster
            modelBuilder.Entity<UserAddressMaster>().HasKey(a => a.AddressID);
            modelBuilder.Entity<UserAddressMaster>()
                .HasOne(a => a.User)
                .WithMany(u => u.Addresses)
                .HasForeignKey(a => a.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            // RestaurantMaster
            modelBuilder.Entity<RestaurantMaster>().HasKey(r => r.RestaurantID);
            modelBuilder.Entity<RestaurantMaster>()
                .HasOne(r => r.User)
                .WithMany(u => u.Restaurants)
                .HasForeignKey(r => r.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            // CategoryMaster
            modelBuilder.Entity<CategoryMaster>().HasKey(c => c.CategoryID);
            modelBuilder.Entity<CategoryMaster>()
                .HasOne(c => c.Restaurant)
                .WithMany(r => r.Categories)
                .HasForeignKey(c => c.RestaurantID)
                .OnDelete(DeleteBehavior.Restrict);

            // MenuItemMaster
            modelBuilder.Entity<MenuItemMaster>().HasKey(m => m.MenuItemID);
            modelBuilder.Entity<MenuItemMaster>()
                .HasOne(m => m.Restaurant)
                .WithMany(r => r.MenuItems)
                .HasForeignKey(m => m.RestaurantID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<MenuItemMaster>()
                .HasOne(m => m.Category)
                .WithMany(c => c.MenuItems)
                .HasForeignKey(m => m.CategoryID)
                .OnDelete(DeleteBehavior.Restrict);

            // DiscountMaster
            modelBuilder.Entity<DiscountMaster>().HasKey(d => d.DiscountID);
            modelBuilder.Entity<DiscountMaster>()
                .HasOne(d => d.MenuItem)
                .WithMany(m => m.Discounts)
                .HasForeignKey(d => d.MenuItemID)
                .OnDelete(DeleteBehavior.Restrict);

            // CartTransaction
            modelBuilder.Entity<CartTransaction>().HasKey(c => c.CartID);
            modelBuilder.Entity<CartTransaction>()
                .HasOne(c => c.User)
                .WithMany(u => u.CartTransactions)
                .HasForeignKey(c => c.UserID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CartTransaction>()
                .HasOne(c => c.MenuItem)
                .WithMany(m => m.Carts)
                .HasForeignKey(c => c.MenuItemID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CartTransaction>()
                .HasIndex(c => new { c.UserID, c.MenuItemID })
                .IsUnique();

            // OrderTransaction
            modelBuilder.Entity<OrderTransaction>().HasKey(o => o.OrderID);
            modelBuilder.Entity<OrderTransaction>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<OrderTransaction>()
                .HasOne(o => o.Restaurant)
                .WithMany(r => r.Orders)
                .HasForeignKey(o => o.RestaurantID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<OrderTransaction>()
                .HasOne(o => o.ShippingAddress)
                .WithMany(a => a.Orders)
                .HasForeignKey(o => o.ShippingAddressID)
                .OnDelete(DeleteBehavior.Restrict);

            // OrderItemDetails
            modelBuilder.Entity<OrderItemDetails>().HasKey(oi => oi.OrderItemID);
            modelBuilder.Entity<OrderItemDetails>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<OrderItemDetails>()
                .HasOne(oi => oi.MenuItem)
                .WithMany(m => m.OrderItems)
                .HasForeignKey(oi => oi.MenuItemID)
                .OnDelete(DeleteBehavior.Restrict);

            // PaymentTransaction
            modelBuilder.Entity<PaymentTransaction>().HasKey(p => p.PaymentID);
            modelBuilder.Entity<PaymentTransaction>()
                .HasOne(p => p.Order)
                .WithOne(o => o.Payment)
                .HasForeignKey<PaymentTransaction>(p => p.OrderID)
                .OnDelete(DeleteBehavior.Restrict);

            // DeliveryStatusTransaction
            modelBuilder.Entity<DeliveryStatusTransaction>().HasKey(d => d.StatusID);
            modelBuilder.Entity<DeliveryStatusTransaction>()
                .HasOne(d => d.Order)
                .WithMany(o => o.DeliveryStatuses)
                .HasForeignKey(d => d.OrderID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<DeliveryStatusTransaction>()
                .HasOne(d => d.User)
                .WithMany(u => u.DeliveryStatuses)
                .HasForeignKey(d => d.StatusUpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // ReviewTransaction
            modelBuilder.Entity<ReviewTransaction>().HasKey(r => r.ReviewID);
            modelBuilder.Entity<ReviewTransaction>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ReviewTransaction>()
                .HasOne(r => r.MenuItem)
                .WithMany(m => m.Reviews)
                .HasForeignKey(r => r.MenuItemID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

