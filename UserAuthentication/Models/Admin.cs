namespace UserAuthentication.Models
{
    public class Admin
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public byte [] PasswordHash { get; set; }

        public byte [] PasswordSlot { get; set; }

        public DateTime TokenCraete { get; set; }

        public DateTime TokenExpier { get; set; }

        public int permision { get; set; }

        public string Eamil { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty; 
    }
}
