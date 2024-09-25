using CompanyManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace CompanyManagement.Models
{
    public class CMScontext : DbContext
    {
        public CMScontext(DbContextOptions<CMScontext> options)
             : base(options)
        {
        }
        public CMScontext()
        {

        }

        public DbSet<Company> Company { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<CompanyProduct> CompanyProduct { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Ticket> Ticket { get; set; }
        public DbSet<ProductType> ProductType { get; set; }
        public DbSet<ServiceType> ServiceType { get; set; }
        public DbSet<UserRole> UserRole { get; set; }
        public DbSet<TicketType> TicketType { get; set; }
        public DbSet<UserProduct> UserProduct { get; set; }
        public DbSet<ProductService> ProductService { get; set; }
        public DbSet<TicketChat> TicketChat { get; set; }
        public DbSet<RoleMenu> RoleMenu { get; set; }
        public DbSet<MenuItem> MenuItem { get; set; }
        public DbSet<TicketUser> TicketUser { get; set; }




    }
}
