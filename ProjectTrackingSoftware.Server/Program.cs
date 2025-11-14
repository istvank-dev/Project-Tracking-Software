using Microsoft.EntityFrameworkCore;
using ProjectTrackingSoftware.Server.Data;
using Microsoft.AspNetCore.Identity;
using ProjectTrackingSoftware.Server.Models;
using ProjectTrackingSoftware.Server.Endpoints;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "DataSource=app.db;Cache=Shared";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Configure Identity
builder.Services.AddIdentityApiEndpoints<ApplicationUser>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
// FIX: Configure the cookie for cross-site/cross-port development
.Services.ConfigureApplicationCookie(options =>
{
    // Setting SameSite=None and Secure=true is necessary for cookies 
    // to be sent in modern browsers during cross-site requests, 
    // which includes different ports on localhost.
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// (login, logout, register, user)
app.MapAuthEndpoints();

app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();