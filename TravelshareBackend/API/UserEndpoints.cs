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
    public static class UserEndpoints
    {
        public static void MapUserEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/users", async (User user, AppDbContext db) =>
            {
                // Check if a user with the same email already exists
                var existingUser = await db.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
                if (existingUser != null)
                {
                    return Results.Conflict("A user with this email already exists.");
                }

                // Hash the password before saving
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

                db.Users.Add(user);
                await db.SaveChangesAsync();

                return Results.Created($"/api/users/{user.Id}", user);
            }).WithName("CreateUser");



            // DELETE: /api/users/{id}
            app.MapDelete("/api/users/{id}", async (int id, AppDbContext db) =>
            {
                var user = await db.Users.FindAsync(id);
                if (user is null) return Results.NotFound();

                db.Users.Remove(user);
                await db.SaveChangesAsync();
                return Results.NoContent();
            }).WithName("DeleteUser");


            //Login: /api/auth/login
            app.MapPost("/api/auth/login", async (LoginRequest loginRequest, AppDbContext db, JwtService jwtService) =>
            {
                var user = await db.Users.FirstOrDefaultAsync(u => u.Email == loginRequest.Email);
                
                // Check if the user exists and verify the password
                if (user is null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password))
                {
                    return Results.Unauthorized();
                }

                var fullName = $"{user.FirstName} {user.LastName}";
                var token = jwtService.GenerateToken(user.Id, fullName);

                return Results.Ok(new { Token = token });
            }).WithName("Login");
        }
    }
}