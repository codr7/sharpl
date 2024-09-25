namespace Sharpl.Ops;

public class Decrement : Op
{
    public static Op Make(Register target, int delta) => new Decrement(target, delta);
    public readonly Register Target;
    public readonly int Delta;
    
    public Decrement(Register target, int delta)
    {
        Target = target;
        Delta = delta;
    }

    public OpCode Code => OpCode.Decrement;

    public string Dump(VM vm) => $"Decrement {Target} {Delta}";
}