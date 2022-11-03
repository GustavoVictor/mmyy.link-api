using Microsoft.EntityFrameworkCore;

public class DomainContext : CommomContext
{
    public DomainContext(DbContextOptions<DevContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.EnableSensitiveDataLogging();
        base.OnConfiguring(options);
    }
}