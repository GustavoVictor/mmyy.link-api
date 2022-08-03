using Microsoft.AspNetCore.Mvc;

public readonly struct ErrorCode : IEquatable<ErrorCode>
{
    /// <summary>
    ///     Http status code
    /// </summary>
    public int Status { get; }

    /// <summary>
    ///     Internal error code
    /// </summary>
    public string Code { get; }

    /// <summary>
    ///     Internal help message
    /// </summary>
    public string Message { get; }

    /// <summary>
    ///     Error message body
    /// </summary>
    public object Body { get; }

    public ErrorCode(int status, string code, string message, object customBody = null)
    {
        Status = status >= 100 ? status : throw new ArgumentOutOfRangeException(nameof(status));
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Message = message ?? throw new ArgumentNullException(nameof(message));
        Body = customBody;
    }

    public override string ToString()
    {
        var body = string.IsNullOrWhiteSpace(Body?.ToString()) ? "null" : $@"""{Body}""";
        return $@"{{""status"":{Status},""code"":""{Code}"",""message"":""{Message}"",""body"":{body}}}";
    }

    #region Custom Parameters

    public ErrorCode FillParameters(params object[] messageParameters)
    {
        return new ErrorCode(Status, Code, string.Format(Message, messageParameters));
    }

    public ErrorCode CustomResponse(object customResponseBody)
    {
        return new ErrorCode(Status, Code, Message, customResponseBody);
    }

    #endregion

    #region Equatable

    public bool Equals(ErrorCode other)
    {
        return Status == other.Status &&
                Code == other.Code &&
                Message == other.Message;
    }

    public override bool Equals(object obj)
    {
        return obj is ErrorCode other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Status, Code, Message);
    }

    public static bool operator ==(ErrorCode left, ErrorCode right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ErrorCode left, ErrorCode right)
    {
        return !left.Equals(right);
    }

    #endregion

    #region ActionResult

    public static implicit operator ErrorCode(ErrorException errorException)
    {
        return errorException.ErrorCode;
    }

    public static implicit operator ErrorCode(Exception exception) => ErrorCode.InternalServerError;

    public ActionResult ToActionResult()
    {
        return this;
    }

    public static implicit operator ActionResult(ErrorCode errorCode)
    {
        return new ObjectResult(errorCode) { StatusCode = errorCode.Status };
    }

    public static implicit operator ActionResult<object>(ErrorCode errorCode)
    {
        return new ObjectResult(errorCode) { StatusCode = errorCode.Status };
    }

    public static implicit operator ActionResult<IEnumerable<object>>(ErrorCode errorCode)
    {
        return new ObjectResult(errorCode) { StatusCode = errorCode.Status };
    }

    #endregion

    #region Error codes

    public static readonly ErrorCode NoError = new ErrorCode(200, "AA0000", "No Error.");

    // Authorization error codes 00
    public static readonly ErrorCode AuthorizationUnauthorized = new ErrorCode(401, "EC0001", "Unauthorized user access.");
    public static readonly ErrorCode AuthorizationProfileUnauthorized = new ErrorCode(401, "EC0002", "Unauthorized user profile access.");

    // Users
    public static readonly ErrorCode UserNickNameIsAlreadyInUse = new ErrorCode(400, "EC1201", "The nickname is already in use!");
    public static readonly ErrorCode UserInvalidUser = new ErrorCode(400, "EC1202", "Invalid user!");
    public static readonly ErrorCode UserInvalidUserName = new ErrorCode(400, "EC1202", "Invalid user name!");
    public static readonly ErrorCode UserInvalidUserLastName = new ErrorCode(400, "EC1202", "Invalid user last name!");
    public static readonly ErrorCode UserInvalidUserEmail = new ErrorCode(400, "EC1202", "Invalid user email!");
    public static readonly ErrorCode UserInvalidUserPassword = new ErrorCode(400, "EC1202", "Invalid user password!");
    public static readonly ErrorCode UserTheCodeIsNotValid = new ErrorCode(400, "EC1203", "The code is not valid!");
    public static readonly ErrorCode UserErroWhileUpdating = new ErrorCode(500, "EC1204", "Error while updating.");

    // Groups
    public static readonly ErrorCode GroupNotFound = new ErrorCode(400, "EC1301", "Group not found!");

    // Cam
    public static readonly ErrorCode CamNoDetectedObjsFound = new ErrorCode(404, "EC1401", "No detected objects found.");
    public static readonly ErrorCode CamUserNotFoundWhileCreateCam = new ErrorCode(404, "EC1402", "User not found while create cam.");

    //ConnectionInfo
    public static readonly ErrorCode ConnectionInfoDomainIsEmpty = new ErrorCode(400, "EC1501", "Connection Info domain is empty.");
    public static readonly ErrorCode ConnectionInfoUserIsEmpty = new ErrorCode(400, "EC1501", "Connection Info user is empty.");
    public static readonly ErrorCode ConnectionInfoPasswordIsEmpty = new ErrorCode(400, "EC1501", "Connection Info password is empty.");
    public static readonly ErrorCode ConnectionInfoUrlIsEmpty = new ErrorCode(400, "EC1501", "Connection Info url is empty.");

    // Role
    public static readonly ErrorCode RoleCodeIsEmpty = new ErrorCode(400, "EC1601", "Role code is empty.");
    public static readonly ErrorCode RoleAlreadyExists = new ErrorCode(400, "EC1601", "Role already exists.");
    public static readonly ErrorCode RoleNotFound = new ErrorCode(400, "EC1601", "Role not found.");
    public static readonly ErrorCode RoleDescriptionIsEmpty = new ErrorCode(400, "EC1601", "Role description is empty.");

    // Others
    public static readonly ErrorCode UserNotFound = new ErrorCode(404, "EC9901", "User not found!");
    public static readonly ErrorCode ProfileNotFound = new ErrorCode(404, "EC9902", "Profile not found!");
    public static readonly ErrorCode PermissionNotFound = new ErrorCode(404, "EC9903", "Permission not found!");

    public static readonly ErrorCode InternalServerError = new ErrorCode(500, "EC9904", "Please, try again!");
    #endregion
}
