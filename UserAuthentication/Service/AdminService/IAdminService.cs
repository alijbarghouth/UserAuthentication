using UserAuthentication.Models;

namespace UserAuthentication.Service.AdminService
{
    public interface IAdminService
    {
        Task<Admin> RegisterAdmin(Admin admin);


        Task<Admin> getName(string name );

        Task<IEnumerable<Admin>> getAllAdmin();
    }
}
