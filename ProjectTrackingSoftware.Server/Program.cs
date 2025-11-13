using Microsoft.EntityFrameworkCore;
using ProjectTrackingSoftware.Server.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "DataSource=app.db;Cache=Shared";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Configure Identity
builder.Services.AddIdentityApiEndpoints<IdentityUser>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

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

// Map identity endpoints - this creates the standard endpoints
var authGroup = app.MapGroup("/api/auth");

// Map all identity endpoints EXCEPT register
authGroup.MapIdentityApi<IdentityUser>();

// Now completely override the register endpoint with our own
authGroup.MapPost("/register", async (
    UserManager<IdentityUser> userManager,
    [FromBody] RegisterModel model) =>
{
    if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.Username))
    {
        return Results.BadRequest("Username, email and password are required.");
    }

    var user = new IdentityUser { UserName = model.Username, Email = model.Email };
    var result = await userManager.CreateAsync(user, model.Password);

    if (result.Succeeded)
    {
        return Results.Ok(new { message = "Registration successful" });
    }

    return Results.BadRequest(result.Errors);
});

// Custom endpoint to get user info
app.MapGet("/api/user", async (UserManager<IdentityUser> userManager, HttpContext httpContext) =>
{
    if (httpContext.User.Identity?.IsAuthenticated == true)
    {
        var user = await userManager.GetUserAsync(httpContext.User);
        if (user != null)
        {
            return Results.Ok(new { user.UserName, user.Email });
        }
    }
    return Results.Unauthorized();
}).RequireAuthorization();

app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();

// DTO for registration
public class RegisterModel
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}