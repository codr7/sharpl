namespace Sharpl;

public class EmitError : Exception
{
    public EmitError(Loc loc, string message) : base($"Emit error in {loc}:\n{message}") { }
}

public class EvalError : Exception
{
    public EvalError(Loc loc, string message) : base($"Eval error in {loc}:\n{message}") { }
}