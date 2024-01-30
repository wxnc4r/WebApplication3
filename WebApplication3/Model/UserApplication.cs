using Microsoft.AspNetCore.Identity;
using WebApplication3.Model;
using WebApplication3.ViewModels;

namespace WebApplication3.Model
{
    public class UserApplication : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string NRIC { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string ResumeFilePath { get; set; }
        public string WhoAmI { get; set; }
    }
}
