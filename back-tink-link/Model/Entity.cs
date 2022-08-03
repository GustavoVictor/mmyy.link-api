public class Entity
{
    public Entity()
    {
        Id = Guid.NewGuid();
        CreatedDate = DateTime.Now;
    }

    public Guid Id { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? LastModified { get; set; }
}