using Microsoft.AspNetCore.Identity;

namespace WebApplication2.Models
{
    public class AspNetUsers:IdentityUser
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
