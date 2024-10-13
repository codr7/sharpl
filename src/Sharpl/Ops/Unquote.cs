namespace Sharpl.Ops;

public class Unquote : Op
{
    public static Op Make(Register target, Loc loc) => new Unquote(target, loc);
    public readonly Register Target;
    public readonly Loc Loc;
    public Unquote(Register target, Loc loc)
    {
        Target = target;
        Loc = loc;
    }

    public OpCode Code => OpCode.Unquote;
    public string Dump(VM vm) => $"UnquoteRegister {Loc} {Target}";
}