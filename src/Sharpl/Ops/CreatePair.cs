namespace Sharpl.Ops;

public class CreatePair : Op
{
    public static Op Make(Register target, Register left, Register right, Loc loc) => 
        new CreatePair(target, left, right, loc);

    public readonly Register Target;
    public readonly Register Left;
    public readonly Register Right;
    public readonly Loc Loc;

    public CreatePair(Register target, Register left, Register right, Loc loc)
    {
        Target = target;
        Left = left;    
        Right = right;
        Loc = loc;
    }

    public OpCode Code => OpCode.CreatePair;
    public string Dump(VM vm) => $"CreatePair {Target} {Left} {Right} {Loc}";
}