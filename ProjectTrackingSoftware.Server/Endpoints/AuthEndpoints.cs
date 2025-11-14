using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectTrackingSoftware.Server.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;
using System.Linq; // Added for the Select and Join extensions

namespace ProjectTrackingSoftware.Server.Endpoints
{
    public static class AuthEndpoints
    {
        public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var authGroup = app.MapGroup("/api/auth");

            // Custom LOGIN endpoint
            authGroup.MapPost("/login", async (
                [FromServices] SignInManager<ApplicationUser> signInManager,
                [FromBody] LoginModel model) =>
            {
                var user = await signInManager.UserManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return Results.Unauthorized();
                }

                // PasswordSignInAsync uses the user's UserName, so we get it from the user object
                var result = await signInManager.PasswordSignInAsync(user.UserName!, model.Password, true, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return Results.Ok();
                }

                return Results.Unauthorized();
            });

            // Custom LOGOUT endpoint
            authGroup.MapPost("/logout", async (
                [FromServices] SignInManager<ApplicationUser> signInManager) =>
            {
                await signInManager.SignOutAsync();
                return Results.Ok();
            });

            // Custom REGISTER endpoint
            authGroup.MapPost("/register", async (
                [FromServices] UserManager<ApplicationUser> userManager,
                [FromBody] RegisterModel model) =>
            {
                if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.Username))
                {
                    // Return a clean error object for mandatory fields
                    return Results.BadRequest(new { errors = "Username, email, and password are required." });
                }

                var user = new ApplicationUser { UserName = model.Username, Email = model.Email };
                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Optional: You might want to sign in the user immediately here
                    return Results.Ok(new { message = "Registration successful" });
                }

                // === CRITICAL FIX START ===
                // 1. Extract error messages (e.Description) from the IdentityResult.Errors collection.
                var errorMessages = result.Errors.Select(e => e.Description);
                // 2. Join them into a single, readable string.
                var consolidatedError = string.Join(" ", errorMessages);

                // 3. Return a Bad Request with a clear, consolidated error message object.
                return Results.BadRequest(new { errors = consolidatedError });
                // === CRITICAL FIX END ===
            });

            // Custom endpoint to get user info
            app.MapGet("/api/user", async (
                [FromServices] UserManager<ApplicationUser> userManager,
                HttpContext httpContext) =>
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