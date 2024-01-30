using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net;
using System.Security.Claims;
using WebApplication3.Model;
using WebApplication3.ViewModels;

namespace WebApplication3.Pages
{
	public class LoginModel : PageModel
	{
		[BindProperty]
		public Login LModel { get; set; }
		private UserManager<UserApplication> userManager { get; }

		private readonly SignInManager<UserApplication> signInManager;
        private readonly ILogger<LoginModel> logger;
		private readonly IHttpContextAccessor contxt;
		private readonly IConfiguration _configuration;

		public LoginModel(IConfiguration configuration,
			IHttpContextAccessor httpContextAccessor,
			ILogger<LoginModel> logger,
			SignInManager<UserApplication> signInManager,
			UserManager<UserApplication> userManager)
		{
			this.logger = logger;
            this.signInManager = signInManager;
			contxt = httpContextAccessor;
			this.userManager = userManager;
			_configuration = configuration;
		}
		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				/*
				var recaptchaToken = Request.Form["g-recaptcha-response"];
				bool isValidRecaptcha = await ValidateRecaptcha(recaptchaToken);

				if (!isValidRecaptcha)
				{
					ModelState.AddModelError(string.Empty, "reCAPTCHA verification failed.");
					return Page();
				} */

				var identityResult = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password, LModel.RememberMe, lockoutOnFailure: true);

				if (identityResult.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Account is locked out. Please try again in 10 seconds.");
                    return Page();
                }

                if (identityResult.Succeeded)
				{
					var user = await userManager.FindByEmailAsync(LModel.Email);
					if (user != null)
					{
						var claims = new List<Claim> {
						new Claim(ClaimTypes.Name, user.Email),
						new Claim(ClaimTypes.Email, user.Email),

                        new Claim("A", "B")
						};

						contxt.HttpContext.Session.SetString("Email", user.Email);

						logger.LogInformation("Claims after login:");
                        foreach (var claim in claims)
                        {
                            logger.LogInformation($"{claim.Type}: {claim.Value}");
                        }

                        var i = new ClaimsIdentity(claims, "MyCookieAuth");
						ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(i);
                        logger.LogInformation("ClaimsPrincipal after login: " + claimsPrincipal);
                        foreach (var claim in claimsPrincipal.Claims)
                        {
                            logger.LogInformation($"{claim.Type}: {claim.Value}");
                        }

                        await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);

						return RedirectToPage("Index");
					}
				}
				ModelState.AddModelError(string.Empty, "Invalid login attempt. Please check your email and password.");
			}
			return Page();	
		}
		/*
		private async Task<bool> ValidateRecaptcha(string token)
		{
			var recaptchaSecretKey = _configuration["6LegemEpAAAAAIbLl_CarL2KIg3EdRRfavT1Dr6M"];
			using (var httpClient = new HttpClient())
			{
				var response = await httpClient.GetStringAsync($"https://www.google.com/recaptcha/api/siteverify?secret={recaptchaSecretKey}&response={token}");
				var recaptchaResult = JsonConvert.DeserializeObject<RecaptchaResult>(response);
				return recaptchaResult.success;
			}
		} */
	}
}
