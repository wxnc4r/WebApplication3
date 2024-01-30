using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication3.Model;
using WebApplication3.ViewModels;
using System.Security.Cryptography;
using System.Text;
using WebApplication3.Utilities;

namespace WebApplication3.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly ILogger<RegisterModel> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private UserManager<UserApplication> userManager { get; }
        private SignInManager<UserApplication> signInManager { get; }

        [BindProperty]
        public Register RModel { get; set; }

        public RegisterModel(
            UserManager<UserApplication> userManager,
            SignInManager<UserApplication> signInManager,
            IWebHostEnvironment webHostEnvironment,
            ILogger<RegisterModel> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            ;
        }

        public void OnGet()
        {
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (RModel.Resume != null && RModel.Resume.Length > 0)
                    {
						string[] allowedFileTypes = { ".docx", ".pdf" };
						string fileExtension = Path.GetExtension(RModel.Resume.FileName).ToLowerInvariant();
						if (!allowedFileTypes.Contains(fileExtension))
						{
							ModelState.AddModelError("RModel.Resume", "Only .docx or .pdf files are allowed.");
							return Page();
						}

						Console.WriteLine($"Received Resume File: {RModel.Resume.FileName}");

                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + RModel.Resume.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await RModel.Resume.CopyToAsync(fileStream);
                        }
                        RModel.ResumeFilePath = filePath;

                        _logger.LogInformation($"File uploaded: {RModel.Resume.FileName}");
                        _logger.LogInformation($"File path: {RModel.ResumeFilePath}");
                    }

                    switch (RModel.Gender)
                    {
                        case "0":
                            RModel.Gender = "Male";
                            break;
                        case "1":
                            RModel.Gender = "Female";
                            break;
                        case "2":
                            RModel.Gender = "Other";
                            break;
                        default:
                            RModel.Gender = "NotSpecified";
                            break;
                    }

                    RijndaelManaged cipher = CryptoUtility.GetRijndaelManaged();
                    ICryptoTransform encryptTransform = cipher.CreateEncryptor();

                    string encryptedNRIC = RModel.NRIC;
                    byte[] nonEncrypted = Encoding.UTF8.GetBytes(encryptedNRIC);

                    byte[] encryptedText = encryptTransform.TransformFinalBlock(nonEncrypted, 0, nonEncrypted.Length);
                    string encryptedString = Convert.ToBase64String(encryptedText);
                    Console.WriteLine("Encrypted Text: " + encryptedString);

                    var user = new UserApplication()
                    {
                        UserName = RModel.Email,
                        Email = RModel.Email,
                        FirstName = RModel.FirstName,
                        LastName = RModel.LastName,
                        Gender = RModel.Gender,
                        NRIC = encryptedString,
                        DateOfBirth = RModel.DateOfBirth,
                        ResumeFilePath = RModel.ResumeFilePath,
                        WhoAmI = RModel.WhoAmI,
                    };

                    var result = await userManager.CreateAsync(user, RModel.Password);

                    if (result.Succeeded)
                    {
                        await signInManager.SignInAsync(user, false);
                        return RedirectToPage("Index");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Exception during file upload: {ex.Message}");
                    ModelState.AddModelError("", "Error during file upload. Please try again.");
                }
            }
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    _logger.LogError($"Model validation error: {error.ErrorMessage}");
                }
            }

            return Page();
        }
    }
}
