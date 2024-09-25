namespace Sharpl.Ops;

public class Branch : Op
{
    public static Op Make(Label right, Loc loc) => new Branch(right, loc);
    public readonly Label Right;
    public readonly Loc Loc;

    public Branch(Label right, Loc loc)
    {
        Right = right; 
        Loc = loc;
    }

    public OpCode Code => OpCode.Branch;
    public string Dump(VM vm) => $"Branch {Loc} {Right}";
}