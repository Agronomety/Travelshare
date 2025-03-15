using Microsoft.EntityFrameworkCore;
using TravelshareBackend.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace TravelshareBackend.API
{
    public static class BlogPostEndpoints
    {
        public static void MapBlogPostEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/blogposts/secure", [Authorize] async (AppDbContext db) =>
            {
                return await db.BlogPosts.ToListAsync();
            }).WithName("GetSecureBlogPosts");

            app.MapGet("/api/blogposts/{id}", async (int id, AppDbContext db) =>
            {
                var blogPost = await db.BlogPosts
                    .Include(bp => bp.User)
                    .Include(bp => bp.Tags)
                    .Include(bp => bp.Comments)
                    .Include(bp => bp.Likes)
                    .FirstOrDefaultAsync(bp => bp.Id == id);
                return blogPost is not null ? Results.Ok(blogPost) : Results.NotFound();
            }).WithName("GetBlogPostById");

            app.MapPost("/api/blogposts", async (HttpContext httpContext, AppDbContext db) =>
            {
                var user = httpContext.User;
                if (user.Identity == null || !user.Identity.IsAuthenticated)
                {
                    return Results.Unauthorized();
                }

                var form = await httpContext.Request.ReadFormAsync();
                var title = form["title"];
                var content = form["content"];
                var file = form.Files["image"];

                if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(content))
                {
                    return Results.BadRequest("Title and content are required.");
                }

                var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Results.BadRequest("Invalid user ID.");
                }

                byte[]? imageBytes = null;
                if (file != null)
                {
                    using var memoryStream = new MemoryStream();
                    await file.CopyToAsync(memoryStream);
                    imageBytes = memoryStream.ToArray();
                }

                var blogPost = new BlogPost
                {
                    Title = title,
                    Content = content,
                    UserId = userId,
                    CreatedDate = DateTime.UtcNow,
                    Image = imageBytes
                };

                db.BlogPosts.Add(blogPost);
                await db.SaveChangesAsync();

                return Results.Created($"/api/blogposts/{blogPost.Id}", blogPost);
            }).RequireAuthorization();

            app.MapPut("/api/blogposts/{id}", async (int id, BlogPost updatedBlogPost, AppDbContext db) =>
            {
                var blogPost = await db.BlogPosts.FindAsync(id);
                if (blogPost is null) return Results.NotFound();

                blogPost.Title = updatedBlogPost.Title;
                blogPost.Content = updatedBlogPost.Content;
                blogPost.Tags = updatedBlogPost.Tags;

                await db.SaveChangesAsync();
                return Results.NoContent();
            }).WithName("UpdateBlogPost");

            app.MapDelete("/api/blogposts/{id}", async (int id, AppDbContext db) =>
            {
                var blogPost = await db.BlogPosts.FindAsync(id);
                if (blogPost is null) return Results.NotFound();

                db.BlogPosts.Remove(blogPost);
                await db.SaveChangesAsync();
                return Results.NoContent();
            }).WithName("DeleteBlogPost");


            app.MapPost("/api/blogposts/{postId}/like", async (int postId, HttpContext httpContext, AppDbContext db) =>
            {
                var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Results.Unauthorized();
                }

                var post = await db.BlogPosts.Include(p => p.Likes).FirstOrDefaultAsync(p => p.Id == postId);
                if (post == null)
                {
                    return Results.NotFound();
                }

                if (!int.TryParse(userId, out int userIdInt))
                {
                    return Results.Unauthorized();
                } 

                var existingLike = post.Likes.FirstOrDefault(l => l.UserId == userIdInt);
                if (existingLike != null)
                {
                    // Unlike: Remove the like
                    post.Likes.Remove(existingLike);

                }
                else
                {
                    // Like: Add a new like
                    post.Likes.Add(new Like { UserId = userIdInt });
                }

                await db.SaveChangesAsync();
                
                return Results.Ok(new { likeCount = post.Likes.Count });
            });
        }
    }
}