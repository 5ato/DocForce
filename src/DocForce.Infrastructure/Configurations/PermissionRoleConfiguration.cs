using DocForce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocForce.Infrastructure.Configurations;

public class PermissionRoleConfiguration : IEntityTypeConfiguration<PermissionRole>
{
    public void Configure(EntityTypeBuilder<PermissionRole> builder)
    {
        
    }
}
