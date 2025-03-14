

namespace TravelshareBlazor.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public string Password { get; set; }
        
    }
}