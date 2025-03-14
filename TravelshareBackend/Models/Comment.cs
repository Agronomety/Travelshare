using System.Text.Json.Serialization;

namespace TravelshareBackend.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UserId { get; set; }
        public int BlogPostId { get; set; }
    }
}