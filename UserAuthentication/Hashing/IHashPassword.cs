using UserAuthentication.Dto;

namespace UserAuthentication.Hashing
{
    public interface IHashPassword
    {
        void CraeteHashPassword(string password, out byte[] passwordHash, out byte[] passwordSlot);

        bool varifyPassword(string password,byte[] passwordHash, byte[] passwordSlot);

        bool varifyAdmin();
    }
}
