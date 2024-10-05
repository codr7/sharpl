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
    public readonly Value Value;

    public UserError(VM vm, Value value, Loc loc) : base(value.Say(vm), loc)
    {
        Value = value;
    }
}