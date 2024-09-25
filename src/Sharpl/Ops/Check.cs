namespace Sharpl.Ops;

public class Check : Op
{
    public static Op Make(Loc loc) => new Check(loc);
    public readonly Loc Loc;

    public Check(Loc loc) : base(OpCode.Check)
    {
        Loc = loc;
    }

    public override string Dump(VM vm) => $"Check {Loc}";
}