using Add_Database_Model.Models;
using Microsoft.EntityFrameworkCore;
using UserAuthentication.Models;

namespace UserAuthentication.Service.AdminService
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDBContext _context;



        public AdminService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Admin> RegisterAdmin(Admin admin)
        {
             await _context.AddAsync(admin);
            _context.SaveChanges();
             return admin;
        }
    

        public async  Task<Admin> getName(string name)
        {
            var names =await _context.Admins.SingleOrDefaultAsync(x => x.Name == name);


            return names;
        }
    }
}
