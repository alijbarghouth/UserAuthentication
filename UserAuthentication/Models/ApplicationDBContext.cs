using Microsoft.EntityFrameworkCore;
using System.Data;
using UserAuthentication.Models;

namespace Add_Database_Model.Models
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext>  options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

        }


        public DbSet<Admin> Admins { get; set; }

    }
}
