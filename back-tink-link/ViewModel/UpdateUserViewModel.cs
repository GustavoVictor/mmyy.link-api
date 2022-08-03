public class UpdateUserViewModel
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string Password { get; set; }

    public string Email { get; set; }

    public User ToEntity()
    {
        return new User
        {
            Id = string.IsNullOrEmpty(Id) ? Guid.NewGuid() : Guid.Parse(Id),
            Name = Name
        };
    }
}
