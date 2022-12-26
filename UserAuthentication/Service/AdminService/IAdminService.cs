using UserAuthentication.Models;

namespace UserAuthentication.Service.AdminService
{
    public interface IAdminService
    {
        Task<Admin> RegisterAdmin(Admin admin);


        Task<Admin> getByName(string name );


        Task<Admin> getByEmail(string Email );


        Task<Admin> getById(int id );


        Task<IEnumerable<Admin>> getAllAdmin();
    }
}
