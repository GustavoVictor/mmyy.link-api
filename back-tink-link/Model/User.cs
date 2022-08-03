public class User : Entity
{
    public string Name { get; set; }

    public string NickName { get;set; }

    public string LastName { get; set; } 

    public string Password { get; set; }

    public string Email { get; set; }

    public int EmailValidationCode { get; set; }

    public bool ValidatedEmail { get; set; }

    public string Roles { get; set; }

    public List<Card> Cards { get; set; }
}