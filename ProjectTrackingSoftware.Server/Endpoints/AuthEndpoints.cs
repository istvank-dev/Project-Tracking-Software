using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectTrackingSoftware.Server.Models;

namespace ProjectTrackingSoftware.Server.Endpoints
{
    public static class AuthEndpoints
    {
        // This is an extension method for IEndpointRouteBuilder
        // It allows us to add all our auth-related endpoints in one call
        public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var authGroup = app.MapGroup("/api/auth");

            // Custom LOGIN endpoint
            authGroup.MapPost("/login", async (
                SignInManager<IdentityUser> signInManager,
                [FromBody] LoginModel model) =>
            {
                var user = await signInManager.UserManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return Results.Unauthorized();
                }

                var result = await signInManager.PasswordSignInAsync(user.UserName, model.Password, true, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return Results.Ok();
                }

                return Results.Unauthorized();
            });

            // Custom LOGOUT endpoint
            authGroup.MapPost("/logout", async (SignInManager<IdentityUser> signInManager) =>
            {
                await signInManager.SignOutAsync();
                return Results.Ok();
            });

            // Custom REGISTER endpoint
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
                        return Results.Ok(new { username = user.UserName, email = user.Email });
                    }
                }
                return Results.Unauthorized();
            }).RequireAuthorization();

            return app;
        }
    }
}