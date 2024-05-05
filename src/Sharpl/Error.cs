namespace Sharpl;

public class EmitError : Exception
{
    public EmitError(Loc loc, string message) : base($"{loc} {message}") { }
}

public class EvalError : Exception
{
    public EvalError(Loc loc, string message) : base($"{loc} {message}") { }
}

public class ReadError : Exception
{
    public ReadError(Loc loc, string message) : base($"{loc} {message}") { }
}