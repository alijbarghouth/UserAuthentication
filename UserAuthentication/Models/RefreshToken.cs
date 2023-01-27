using Microsoft.EntityFrameworkCore;
using UserAuthentication.Models;

namespace UserAuthentication.Token
{
    public class RefreshToken
    {
        
        public int Id { get; set; }
        
        public int AdminId { get; set; }

        public string Token { get; set; } = string.Empty;

        public DateTime Created { get; set; } = DateTime.Now;


        public DateTime Expires { get; set; }

        public Admin Admin { get; set; }
    }
}
