namespace Sharpl.Ops;

public class Decrement : Op
{
    public static Op Make(Register target, int delta) => new Decrement(target, delta);
    public readonly Register Target;
    public readonly int Delta;
    
    public Decrement(Register target, int delta): base(OpCode.Decrement)
    {
        Target = target;
        Delta = delta;
    }

    public override string Dump(VM vm) => $"Decrement {Target} {Delta}";
}