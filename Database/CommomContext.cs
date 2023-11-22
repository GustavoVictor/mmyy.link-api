using Microsoft.EntityFrameworkCore;

public class CommomContext : DbContext
{
    #region DbSets
    public DbSet<User> Users { get; set; } 
    #endregion

    public CommomContext(DbContextOptions options) : base(options){}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DomainContext).Assembly);

        //Configurações globais
        ApplyGlobalStandards(modelBuilder);

        //User
        modelBuilder.Entity<User>().ToTable("USERS");
        modelBuilder.Entity<User>().HasKey(user => user.Id);
        modelBuilder.Entity<User>().Property(user => user.Name).HasMaxLength(50).IsRequired();
        modelBuilder.Entity<User>().Property(user => user.NickName).HasMaxLength(30).IsRequired();
        modelBuilder.Entity<User>().Property(user => user.LastName).HasMaxLength(50).IsRequired();
        modelBuilder.Entity<User>().Property(user => user.Password).HasMaxLength(50).IsRequired();
        modelBuilder.Entity<User>().Property(user => user.Email).HasMaxLength(200).IsRequired();
        modelBuilder.Entity<User>().Property(user => user.Roles).HasMaxLength(1200).IsRequired();

        //Card
        modelBuilder.Entity<Card>().ToTable("CARDS");
        modelBuilder.Entity<Card>().HasKey(card => card.Id);
        modelBuilder.Entity<Card>().Property(card => card.Index).HasMaxLength(3).IsRequired();
        modelBuilder.Entity<Card>().Property(card => card.Icon).HasMaxLength(70).IsRequired(false);
        modelBuilder.Entity<Card>().Property(card => card.Group).HasMaxLength(70).IsRequired(false);
        modelBuilder.Entity<Card>().Property(card => card.Description).HasMaxLength(200);
        modelBuilder.Entity<Card>().Property(card => card.Social);
        modelBuilder.Entity<Card>().Property(card => card.URL).HasMaxLength(800);
        modelBuilder.Entity<Card>().HasOne<User>(card => card.User).WithMany(user => user.Cards).HasForeignKey(card => card.UserId);

        Seed(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        OnBeforeSaving();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
    {
        OnBeforeSaving();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void OnBeforeSaving()
    {
        ChangeTracker.Entries().ToList().ForEach(entry =>
        {
            if (entry.Entity is Entity trackableEntity)
            {
                if (entry.State == EntityState.Added)
                {
                    trackableEntity.CreatedDate = DateTime.Now;
                }

                else if (entry.State == EntityState.Modified)
                    trackableEntity.LastModified = DateTime.Now;
            }
        });
    }

    public ModelBuilder ApplyGlobalStandards(ModelBuilder builder)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                switch (property.Name)
                {
                    case nameof(Entity.Id):
                        property.IsKey();
                        break;
                    case nameof(Entity.LastModified):
                        property.IsNullable = true;
                        break;
                    case nameof(Entity.CreatedDate):
                        property.IsNullable = false;
                        property.SetDefaultValueSql("GETDATE()");
                        break;
                }
            }
        }

        return builder;
    }

    private void Seed(ModelBuilder build)
    {
        // build.Entity<Account>().HasData(
        //     new Account{ Id = 1, Amount = 500.00M },     
        //     new Account{ Id = 2, Amount = 1000.00M },     
        //     new Account{ Id = 3, Amount = 1500.00M }     
        // );
    }
}