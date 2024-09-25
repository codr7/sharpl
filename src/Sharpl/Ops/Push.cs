namespace Sharpl.Ops;

public class Push : Op
{
    public static Op Make(Value value) => new Push(value);
    public readonly Value Value;
    public Push(Value value): base(OpCode.Push)
    {
        Value = value;
    }

    public override string Dump(VM vm) => $"Push {Value}";
}