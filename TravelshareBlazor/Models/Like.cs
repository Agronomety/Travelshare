using System.Text.Json.Serialization;

namespace TravelshareBlazor.Models
{
    public class Like
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        [JsonIgnore]
        public User User { get; set; }
        public int BlogPostId { get; set; }

        [JsonIgnore]
        public BlogPost BlogPost { get; set; }
    
    }
}