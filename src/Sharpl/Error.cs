namespace Sharpl;

public class EmitError : Exception
{
    public EmitError(string message, Loc loc) : base($"{loc} {message}") { }
}

public class EvalError : Exception
{
    public EvalError(string message, Loc loc) : base($"{loc} {message}") { }
}

public class ReadError : Exception
{
    public ReadError(string message, Loc loc) : base($"{loc} {message}") { }
}
public class UserError : EvalError
{
    public readonly Type<UserError> Type;

    public UserError(Type<UserError> type, string message, Loc loc) : base(message, loc)
    {
        Type = type;
    }
}