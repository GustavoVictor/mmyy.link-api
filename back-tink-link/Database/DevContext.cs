using Microsoft.EntityFrameworkCore;

public class DevContext : CommomContext
{
    public DevContext(DbContextOptions<DevContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.EnableSensitiveDataLogging();
        base.OnConfiguring(options);
    }
}