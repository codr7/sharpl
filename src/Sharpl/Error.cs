namespace Sharpl;

public class Error : Exception
{
    public readonly Loc Loc;

    public Error(string message, Loc loc) : base($"{loc} {message}")
    {
        Loc = loc;
    }
};

public class EmitError : Error
{
    public EmitError(string message, Loc loc) : base(message, loc) { }
}

public class EvalError : Error
{
    public EvalError(string message, Loc loc) : base(message, loc) { }
}

public class ReadError : Error
{
    public ReadError(string message, Loc loc) : base(message, loc) { }
}
public class UserError : EvalError
{
    public readonly Value Value;

    public UserError(VM vm, Value value, Loc loc) : base(value.Say(vm), loc)
    {
        Value = value;
    }
}