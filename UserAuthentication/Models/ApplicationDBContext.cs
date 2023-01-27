using Microsoft.EntityFrameworkCore;
using System.Data;
using UserAuthentication.Models;
using UserAuthentication.Token;

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
        public DbSet<RefreshToken> RefreshTokens { get; set; }

    }
}
