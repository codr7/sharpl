namespace Sharpl.Ops;

public class Branch : Op
{
    public static Op Make(Label right, Loc loc) => new Branch(right, loc);
    public readonly Label Right;
    public readonly Loc Loc;

    public Branch(Label right, Loc loc): base(OpCode.Branch)
    {
        Right = right; 
        Loc = loc;
    }
    
    public override string Dump(VM vm) => $"Branch {Loc} {Right}";
}