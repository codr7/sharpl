namespace Sharpl.Ops;

public class Increment : Op
{
    public static Op Make(Register target, int delta) => new Increment(target, delta);
    public readonly Register Target;
    public readonly int Delta;
    public Increment(Register target, int delta): base(OpCode.Increment)
    {
        Target = target;
        Delta = delta;
    }

    public override string Dump(VM vm) => $"Increment {Target} {Delta}";
}