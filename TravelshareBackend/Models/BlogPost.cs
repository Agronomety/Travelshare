using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelshareBackend.Models
{
    public class BlogPost
{
    public int Id { get; set; }
    
    [Column(TypeName = "varchar(255)")]
    public string Title { get; set; }

     [Column(TypeName = "TEXT")]
    public string Content { get; set; }
    public DateTime CreatedDate { get; set; }
    public int UserId { get; set; }
    public byte[]? Image { get; set; }

    [JsonIgnore]  
    public User User { get; set; }

    public ICollection<Tag> Tags { get; set; }
    public ICollection<Like> Likes { get; set; }
    public ICollection<Comment> Comments { get; set; }
}
}
