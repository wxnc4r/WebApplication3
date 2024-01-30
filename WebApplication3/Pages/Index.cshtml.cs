using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApplication3.Model;
using WebApplication3.Utilities;

namespace WebApplication3.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly RijndaelManaged _rijndael;
        private UserManager<UserApplication> userManager { get; }

        public IndexModel(ILogger<IndexModel> logger, UserManager<UserApplication> userManager)
        {
            _rijndael = CryptoUtility.GetRijndaelManaged();
            _logger = logger;
            this.userManager = userManager;

        }

        public async Task OnGetAsync()
        {
            foreach (var claim in User.Claims)
            {
                _logger.LogInformation($"{claim.Type}: {claim.Value}");
            }
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await userManager.FindByEmailAsync(email);

            string nricEncrypted = user.NRIC;

            RijndaelManaged cipher = _rijndael;
            ICryptoTransform decryptTransform = cipher.CreateDecryptor();

            byte[] cipherText = Convert.FromBase64String(nricEncrypted);
            byte[] decryptedText = decryptTransform.TransformFinalBlock(cipherText, 0, cipherText.Length);
            string decryptedString = Encoding.UTF8.GetString(decryptedText);

            ViewData["Email"] = User.FindFirstValue(ClaimTypes.Email);
            ViewData["NRIC"] = decryptedString;
            ViewData["ResumeFilePath"] = user.ResumeFilePath;
            ViewData["Gender"] = user.Gender;
            ViewData["Name"] = user.FirstName + " " + user.LastName;
            ViewData["DOB"] = user.DateOfBirth.ToString("MM/dd/yyyy");
            ViewData["AboutYou"] = user.WhoAmI;
        }
    }
}