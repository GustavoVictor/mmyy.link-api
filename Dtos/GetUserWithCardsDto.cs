public struct GetUserWithCardsDto
{   
    public string Name { get; set; }

    public string NickName { get; set; }

    public string LastName { get; set; } 

    public string Summary { get; set; }
    
    public string BackgroundColor { get; set; }

    public string BackgroundImage { get; set; }

    public string Email { get; set; }

    public GetCardDto[] Cards { get; set; }
}