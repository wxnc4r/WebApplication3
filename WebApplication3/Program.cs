using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using WebApplication3.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<AuthDbContext>();
builder.Services.AddIdentity<UserApplication, IdentityRole>(options =>
{
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(10);
})
    .AddEntityFrameworkStores<AuthDbContext>();

builder.Services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth", options =>
{
    options.Cookie.Name = "MyCookieAuth";
	options.LoginPath = "/Login/Index";
	options.AccessDeniedPath = "/Account/Denied";
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MustBelongToIndex",
        policy => policy.RequireClaim("A", "B"));
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(30);
});

builder.Services.ConfigureApplicationCookie(config =>
{
    config.AccessDeniedPath = "/Account/Denied";
    config.LoginPath = "/Login";
});

builder.Services.Configure<Recaptcha>(builder.Configuration.GetSection("Recaptcha"));

builder.Logging.AddConsole(); 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithRedirects("/Errors/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseSession();

app.MapRazorPages();

app.Run();
