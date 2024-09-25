namespace Sharpl.Ops;

public class Check : Op
{
    public static Op Make(Loc loc) => new Check(loc);
    public readonly Loc Loc;

    public Check(Loc loc)
    {
        Loc = loc;
    }

    public OpCode Code => OpCode.Check;
    public string Dump(VM vm) => $"Check {Loc}";
}