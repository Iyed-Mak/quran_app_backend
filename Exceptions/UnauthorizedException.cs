namespace QuranSchool.Api.Exceptions;

public class UnauthorizedException(string message) : ApiException(StatusCodes.Status401Unauthorized, message);
