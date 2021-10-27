namespace WalletPlusIncAPI.Models.Dtos.AppUser
{
    
    public class AppUserReadDto
    {
        public string Id { get; set; }

        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        
        public string UserName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Type { get; set; }

        public string Address { get; set; }
        public string AvatarUrl { get; set; }
        public string Gender { get; set; }
    }
}