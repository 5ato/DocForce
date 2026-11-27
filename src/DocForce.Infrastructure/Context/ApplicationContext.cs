using DocForce.Domain.Entities;
using DocForce.Infrastructure.Common.Outbox;
using DocForce.Share;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DocForce.Infrastructure.Context;

public class ApplicationContext : DbContext
{
    #region Domain Entities
    public DbSet<User> Users => Set<User>();
    public DbSet<Community> Communities => Set<Community>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<MemberRole> MemberRoles => Set<MemberRole>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<PermissionRole> PermissionRoles => Set<PermissionRole>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    #endregion

    #region Infrastructure Entities
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    #endregion


    private readonly IConfiguration _configuration;
    public ApplicationContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString(Constants.DocPostgresConnectionString));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);
    }
}
