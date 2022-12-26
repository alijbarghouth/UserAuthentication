﻿using Add_Database_Model.Models;
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
    

        public async  Task<Admin> getByName(string name)
        {
            var names =await _context.Admins.SingleOrDefaultAsync(x => x.Name == name);




            return names;
        }

        public async Task<IEnumerable<Admin>> getAllAdmin()
        {
            var admin = await _context.Admins.ToListAsync();
            return admin;
        }

        public async Task<Admin> getById(int id)
        {
            var admin = await _context.Admins.FindAsync(id);

            return admin;
        }

        public async Task<Admin> getByEmail(string Email)
        {
            var admin = await _context.Admins.SingleOrDefaultAsync(x => x.Eamil == Email);

            return admin;
        }
    }
}
