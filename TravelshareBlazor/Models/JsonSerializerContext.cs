using System.Text.Json.Serialization;

namespace TravelshareBlazor.Models
{
    [JsonSerializable(typeof(LoginRequest))]
    
    [JsonSerializable(typeof(BlogPost))]
    [JsonSerializable(typeof(User))]
    [JsonSerializable(typeof(Comment))]
    [JsonSerializable(typeof(Like))]
    [JsonSerializable(typeof(Tag))]
    [JsonSerializable(typeof(List<BlogPost>))]
    [JsonSerializable(typeof(List<User>))]
    [JsonSerializable(typeof(List<Comment>))]
    [JsonSerializable(typeof(List<Like>))]
    [JsonSerializable(typeof(List<Tag>))]
    internal partial class AppJsonSerializerContext : JsonSerializerContext
    {
    }
}
