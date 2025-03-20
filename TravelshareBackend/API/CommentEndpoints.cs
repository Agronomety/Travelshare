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
    public static class CommentEndpoints{

        public static void MapCommentEndpoints(this IEndpointRouteBuilder app)
        {   
            
            app.MapGet("/api/blogposts/{postId}/comments", async (int postId, AppDbContext db) =>
            {
                var comments = await db.Comments
                    .Where(c => c.BlogPostId == postId)
                    .OrderByDescending(c => c.CreatedDate)
                    .Select(c => new
                    {
                        c.Id,
                        c.Content,
                        c.CreatedDate,
                        c.UserId,
                        UserFullName = db.Users
                            .Where(u => u.Id == c.UserId)
                            .Select(u => u.FirstName + " " + u.LastName)
                            .FirstOrDefault() ?? "Unknown User" 
                    })
                    .ToListAsync();

                return Results.Ok(comments);
            });



            
            app.MapPost("/api/blogposts/{postId}/comment", async (int postId, Comment comment, AppDbContext db, HttpContext httpContext) =>
            {
                var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null || !int.TryParse(userId, out int userIdInt))
                {
                    return Results.Unauthorized();
                }

                var blogPost = await db.BlogPosts.FindAsync(postId);
                if (blogPost is null)
                {
                    return Results.NotFound();
                }

                if (string.IsNullOrWhiteSpace(comment.Content))
                {
                    return Results.BadRequest("Comment content cannot be empty.");
                }

                comment.UserId = userIdInt;
                comment.BlogPostId = postId;
                comment.CreatedDate = DateTime.UtcNow;

                db.Comments.Add(comment);
                await db.SaveChangesAsync();

                return Results.Created($"/api/blogposts/{postId}/comments/{comment.Id}", new
                {
                    comment.Id,
                    comment.Content,
                    comment.CreatedDate,
                    comment.UserId
                });
            });
        }
    }
}