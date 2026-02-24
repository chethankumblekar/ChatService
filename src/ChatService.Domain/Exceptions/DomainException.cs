namespace ChatService.Domain.Exceptions;

public class DomainException : Exception { public DomainException(string msg) : base(msg) { } }

public class NotFoundException : DomainException
{
    public NotFoundException(string entity, object key) : base($"{entity} '{key}' was not found.") { }
}

public class UnauthorizedException : DomainException
{
    public UnauthorizedException(string msg = "Access denied.") : base(msg) { }
}

public class ValidationException : DomainException
{
    public IDictionary<string, string[]> Errors { get; }
    public ValidationException(IDictionary<string, string[]> errors) : base("Validation failed.") => Errors = errors;
}
