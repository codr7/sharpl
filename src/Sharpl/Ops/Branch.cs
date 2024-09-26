namespace Sharpl.Ops;

public class Branch : Op
{
    public static Op Make(Label right, bool pop, Loc loc) => new Branch(right, pop, loc);
    public readonly Label Right;
    public readonly bool Pop;
    public readonly Loc Loc;

    public Branch(Label right, bool pop, Loc loc)
    {
        Right = right;
        Pop = pop;
        Loc = loc;
    }

    public OpCode Code => OpCode.Branch;
    public string Dump(VM vm) => $"Branch {Right} {Pop} {Loc}";
}