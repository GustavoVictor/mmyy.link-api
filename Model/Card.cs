public class Card : Entity
{
    public int Index { get; set; }

    public string Group { get; set; }

    public string Icon { get; set; }

    public string Description { get; set; }

    public string URL { get; set; }
    
    public bool Social { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; }
}