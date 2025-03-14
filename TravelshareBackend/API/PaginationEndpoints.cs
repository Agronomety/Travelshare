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
    public static class PaginationEndpoints
    {
        public static void MapPaginationEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/blogposts", async (AppDbContext db, string? search, string? orderBy, int page = 1, int pageSize = 10) =>
            {
                var query = db.BlogPosts
                    .Include(b => b.User)
                    .Include(b => b.Tags)
                    .Include(b => b.Likes)
                    .Include(b => b.Comments)
                    .AsQueryable();

                // 📜 Pagination
                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(b =>
                        b.Title.Contains(search) ||
                        b.User.FirstName.Contains(search) ||
                        b.User.LastName.Contains(search) ||
                        b.Tags.Any(t => t.Name.Contains(search))
                    );
                }

                // 📊 Sorting
                query = orderBy switch
                {
                    "likes" => query.OrderByDescending(b => b.Likes.Count),
                    "comments" => query.OrderByDescending(b => b.Comments.Count),
                    _ => query.OrderByDescending(b => b.CreatedDate)
                };

                // 🔍 Search logic (title, user, tags)
                int totalItems = await query.CountAsync();
                var posts = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

                var response = new
                {
                    TotalItems = totalItems,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalItems / pageSize),
                    BlogPosts = posts.Select(p => new
                    {
                        p.Id,
                        p.Title,
                        p.Content,
                        p.CreatedDate,
                        p.UserId,
                        ImageBase64 = p.Image != null ? Convert.ToBase64String(p.Image) : null,
                        p.Likes,
                        p.Comments
                    })
                };

                return Results.Ok(response);
            });
        }
    }

}