using Microsoft.EntityFrameworkCore;               
using Microsoft.EntityFrameworkCore.Metadata.Builders; 
using TravelshareBackend.Models;                 

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(100);
        builder.HasIndex(u => u.Email).IsUnique();
    }
}