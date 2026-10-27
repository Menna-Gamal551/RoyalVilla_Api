using Microsoft.AspNetCore.Identity;

namespace RoyalVilla_Api.Data.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string Name {  get; set; }
    }
}
