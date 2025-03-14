using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

public class BlogPostService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public BlogPostService(HttpClient httpClient, AuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    public async Task<BlogPostPaginationResult> GetBlogPostsAsync(BlogPostRequest request)
    {
        try
        {
            string url = $"/api/blogposts?page={request.Page}&pageSize={request.PageSize}";

            if (!string.IsNullOrWhiteSpace(request.SearchQuery))
            {
                url += $"&search={request.SearchQuery}";
            }
            if (!string.IsNullOrWhiteSpace(request.OrderBy))
            {
                url += $"&orderBy={request.OrderBy}";
            }
            if (request.UserPostsOnly)
            {
                url = $"/api/user/posts?page={request.Page}&pageSize={request.PageSize}";
            }

            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
            
            // Add authorization if needed
            var token = await _authService.GetAuthToken();
            if (!string.IsNullOrEmpty(token))
            {
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.SendAsync(httpRequest);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var paginationResponse = JsonSerializer.Deserialize<PaginationResponse>(responseData, options);

                return new BlogPostPaginationResult
                {
                    BlogPosts = paginationResponse.BlogPosts,
                    TotalPages = paginationResponse.TotalPages,
                    TotalItems = paginationResponse.TotalItems,
                    CurrentPage = paginationResponse.Page,
                    HasNextPage = paginationResponse.Page < paginationResponse.TotalPages,
                    HasPreviousPage = paginationResponse.Page > 1
                };
            }
            else
            {
                Console.WriteLine($"🚨 API Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                return new BlogPostPaginationResult { BlogPosts = new List<BlogPost>() };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Exception in GetBlogPostsAsync: {ex.Message}");
            return new BlogPostPaginationResult { BlogPosts = new List<BlogPost>() };
        }
    }

    public async Task<bool> CreateBlogPostAsync(string title, string content, IBrowserFile file = null)
    {
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content))
        {
            return false;
        }

        try
        {
            var formDataContent = new MultipartFormDataContent
            {
                { new StringContent(title), "title" },
                { new StringContent(content), "content" }
            };

            if (file != null)
            {
                var fileContent = new StreamContent(file.OpenReadStream(5000000));
                formDataContent.Add(fileContent, "image", file.Name);
            }

            var response = await _httpClient.PostAsync("/api/blogposts", formDataContent);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Exception in CreateBlogPostAsync: {ex.Message}");
            return false;
        }
    }

    public async Task<List<Comment>> GetCommentsAsync(int postId)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<Comment>>($"/api/blogposts/{postId}/comments");
            return response ?? new List<Comment>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading comments: {ex.Message}");
            return new List<Comment>();
        }
    }

    public async Task<bool> AddCommentAsync(int postId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return false;
        }

        try
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/blogposts/{postId}/comment", new { Content = content });
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error posting comment: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> ToggleLikeAsync(int postId)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/blogposts/{postId}/like", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error toggling like: {ex.Message}");
            return false;
        }
    }
}

public class BlogPostRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SearchQuery { get; set; } = "";
    public string OrderBy { get; set; } = "";
    public bool UserPostsOnly { get; set; } = false;
}

public class BlogPostPaginationResult
{
    public List<BlogPost> BlogPosts { get; set; } = new();
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
    public int CurrentPage { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}

public class PaginationResponse
{
    public int TotalItems { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public List<BlogPost> BlogPosts { get; set; } = new();
}

public class BlogPost
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string ImageBase64 { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<Like> Likes { get; set; } = new();
    public List<Comment> Comments { get; set; } = new();
}

public class Like
{
    public int Id { get; set; }
    public int UserId { get; set; }
}

public class Comment
{
    public int Id { get; set; }
    public string Content { get; set; }
    public string UserFullName { get; set; }
}