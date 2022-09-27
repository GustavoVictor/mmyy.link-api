public struct UpdateCardDto
{
    public Guid Id { get; set; }
    
    public int Index { get; set; }

    public string Group { get; set; }

    public string Icon { get; set; }

    public string Description { get; set; }

    public string URL { get; set; }
}