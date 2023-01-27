namespace UserAuthentication.Dto
{
    public class AdminDto
    {
        public string Password { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public int permision { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;
    }
}
