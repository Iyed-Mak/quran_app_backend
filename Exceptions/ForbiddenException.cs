namespace QuranSchool.Api.Exceptions;

public class ForbiddenException(string message, string? reason = null) : ApiException(StatusCodes.Status403Forbidden, message)
{
    public string? Reason { get; } = reason;
}
