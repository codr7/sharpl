namespace Sharpl.Ops;

public class Branch : Op
{
    public static Op Make(Register cond, Label right, Loc loc) => new Branch(cond, right, loc);
    public readonly Register Cond;
    public readonly Label Right;
    public readonly Loc Loc;

    public Branch(Register cond, Label right, Loc loc)
    {
        Cond = cond;
        Right = right;
        Loc = loc;
    }

    public OpCode Code => OpCode.Branch;
    public string Dump(VM vm) => $"Branch {Cond} {Right} {Loc}";
}