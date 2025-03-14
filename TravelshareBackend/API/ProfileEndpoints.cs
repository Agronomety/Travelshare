using Microsoft.EntityFrameworkCore;
using TravelshareBackend.Models;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BCrypt.Net;
using System.Security.Claims;

namespace TravelshareBackend.API
{    
    public static class ProfileEndpoints{
        public static void MapProfileEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/user/profile", async (HttpContext context, AppDbContext db) =>
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Results.Unauthorized();
                }

                var user = await db.Users.FindAsync(int.Parse(userId));
                if (user is null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(new { user.FirstName, user.LastName, user.Email });
            }).RequireAuthorization();


            app.MapPut("api/user/password", async (HttpContext context, AppDbContext db) =>
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Results.Unauthorized();
                }

                var user = await db.Users.FindAsync(int.Parse(userId));
                if (user is null)
                {
                    return Results.NotFound();
                }

                var form = await context.Request.ReadFormAsync();
                var currentPassword = form["currentPassword"];
                var newPassword = form["newPassword"];

                if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword))
                {
                    return Results.BadRequest("Current and new password are required.");
                }

                if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.Password))
                {
                    return Results.BadRequest("Invalid current password.");
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                await db.SaveChangesAsync();

                return Results.NoContent();
            }).RequireAuthorization();


            app.MapPut("api/user/firstName", async (HttpContext context, AppDbContext db) =>
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Results.Unauthorized();
                }

                var user = await db.Users.FindAsync(int.Parse(userId));
                if (user is null)
                {
                    return Results.NotFound();
                }

                var form = await context.Request.ReadFormAsync();
                var firstName = form["firstName"];

                if (string.IsNullOrEmpty(firstName))
                {
                    return Results.BadRequest("First name is required.");
                }

                user.FirstName = firstName;
                await db.SaveChangesAsync();

                return Results.NoContent();
            }).RequireAuthorization();

            app.MapPut("api/user/lastName", async (HttpContext context, AppDbContext db) =>
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Results.Unauthorized();
                }

                var user = await db.Users.FindAsync(int.Parse(userId));
                if (user is null)
                {
                    return Results.NotFound();
                }

                var form = await context.Request.ReadFormAsync();
                var lastName = form["lastName"];

                if (string.IsNullOrEmpty(lastName))
                {
                    return Results.BadRequest("Last name is required.");
                }

                user.LastName = lastName;
                await db.SaveChangesAsync();

                return Results.NoContent();
            }).RequireAuthorization();

            app.MapPut("api/user/email", async (HttpContext context, AppDbContext db) =>
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Results.Unauthorized();
                }

                var user = await db.Users.FindAsync(int.Parse(userId));
                if (user is null)
                {
                    return Results.NotFound();
                }

                var form = await context.Request.ReadFormAsync();
                var email = form["email"];

                if (string.IsNullOrEmpty(email))
                {
                    return Results.BadRequest("Email is required.");
                }

                user.Email = email;
                await db.SaveChangesAsync();

                return Results.NoContent();
            }).RequireAuthorization();
        }
    }
}