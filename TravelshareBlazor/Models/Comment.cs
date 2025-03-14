using System.Text.Json.Serialization;

namespace TravelshareBlazor.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UserId { get; set; }
        public int BlogPostId { get; set; }
        public string UserFullName { get; set; } = "Unknown User";
        

       
        
    }
}