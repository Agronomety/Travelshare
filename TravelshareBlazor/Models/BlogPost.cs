using System.Text.Json.Serialization;

namespace TravelshareBlazor.Models
{
    public class BlogPost
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreatedDate { get; set; }
    public int UserId { get; set; }
    public string? ImageBase64 { get; set; }

    [JsonIgnore]  
    public User User { get; set; }

    public ICollection<Tag> Tags { get; set; }
    public ICollection<Like> Likes { get; set; }
    public ICollection<Comment> Comments { get; set; }
}
}
