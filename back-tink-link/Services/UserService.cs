using Microsoft.EntityFrameworkCore;

public class UserService
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Card> _cardRepository;
        
    private readonly AuthService _authService;
    
    private readonly IConfiguration _configuration;

    public UserService(IRepository<User> userRepository,
                IRepository<Card> cardRepository,
                AuthService facadeToken,
                IConfiguration configuration)
    {
        _userRepository = userRepository;
        _cardRepository = cardRepository;
        _authService = facadeToken;
        _configuration = configuration;
    }

    public async Task<string> Auth(AuthViewModel auth)
    {
        var _key = _configuration.GetSection("PasswordKey")?.Get<string>();

        if (_key == null)
            throw new ErrorException(ErrorCode.InternalServerError);

        string _encryptedPassword = await auth.Password.EncryptString(_key);

        User _user = _userRepository.Find(wh => wh.Email == auth.Email
                                                && wh.Password == _encryptedPassword);

        if (_user == null)
            throw new ErrorException(ErrorCode.UserNotFound);

        return _authService.GenerateToken(_user);
    }

    public async Task<dynamic> Create(CreateUserViewModel user)
    {
        var _key = _configuration.GetSection("PasswordKey")?.Get<string>();

        if (_key == null)
            throw new ErrorException(ErrorCode.InternalServerError);

        ValidateUser(user);

        User _user = new User
        {
            NickName = user.NickName,
            Name = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Roles = string.Join("|", user.Roles),
            Password = await user.Password.EncryptString(_key),
            EmailValidationCode = new Random().Next(111111, 999999)
        };

        await _userRepository.CreateAsync(_user);

        var token = await Auth(new AuthViewModel
        {
            Email = user.Email,
            Password = user.Password
        });

        return token;
    }

    private void ValidateUser(CreateUserViewModel user)
    {
        if (user == null)
            throw new ErrorException(ErrorCode.UserInvalidUser);

        if (string.IsNullOrEmpty(user.NickName))
            throw new ErrorException(ErrorCode.UserInvalidUserNickName);

        if (string.IsNullOrEmpty(user.FirstName))
            throw new ErrorException(ErrorCode.UserInvalidUserName);

        if (string.IsNullOrEmpty(user.LastName))
            throw new ErrorException(ErrorCode.UserInvalidUserLastName);

        if (string.IsNullOrEmpty(user.Email))
            throw new ErrorException(ErrorCode.UserInvalidUserEmail);

        if (string.IsNullOrEmpty(user.Password))
            throw new ErrorException(ErrorCode.UserInvalidUserPassword);
    }

    public async Task<dynamic> ValidateCode(Guid userId, int code)
    {
        var user = await _userRepository.GetAsync(wh => wh.Id == userId);

        if (user == null)
            throw new ErrorException(ErrorCode.UserNotFound);

        if (user.EmailValidationCode != code)
            throw new ErrorException(ErrorCode.UserTheCodeIsNotValid);

        user.ValidatedEmail = true;

        var updateResult = await _userRepository.UpdateAsync(user);

        if (!updateResult)
            throw new ErrorException(ErrorCode.UserErroWhileUpdating);

        return new { Message = "Validated email!" };
    }

    public async Task<GetUserWithCardsDto> GetUser(string nickName)
    {
        var user = await _userRepository.GetAsync(wh => wh.NickName == nickName, x => x.Include(i => i.Cards));

        if (user == null)
            throw new ErrorException(ErrorCode.UserNotFound);

        return ToDto(user);
    }

    private GetUserWithCardsDto ToDto(User user)
    {   
        var cardsDto = new GetCardDto[user.Cards.Count];

        for(int i = 0; i < user.Cards.Count; i++){
            var card = user.Cards[i];

            var cardDto = new GetCardDto
            {
                Index = card.Index,
                Group = card.Group,
                Icon = card.Icon,
                Description = card.Description,
                URL = card.URL
            };

            cardsDto[i] = cardDto;
        }

        return new GetUserWithCardsDto
        {
            Name = user.Name,
            NickName = user.NickName,
            LastName = user.LastName,
            Email = user.Email,
            Cards = cardsDto
        };
    }

    public async Task<Card> AddCard(string userId, AddCardDto cardDto){
        
        if (!Guid.TryParse(userId, out Guid parsedUserId))
            throw new ErrorException(ErrorCode.UserNotFound);
        
        var card = new Card{
            Index = cardDto.Index,
            Group = cardDto.Group,
            Icon = cardDto.Icon,
            Description = cardDto.Description,
            URL = cardDto.URL,
            UserId = parsedUserId
        };

        return await _cardRepository.CreateAsync(card);
    }
}