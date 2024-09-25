namespace Sharpl.Ops;

public class Push : Op
{
    public static Op Make(Value value) => new Push(value);
    public readonly Value Value;
    public Push(Value value)
    {
        Value = value;
    }

    public OpCode Code => OpCode.Push;
    public string Dump(VM vm) => $"Push {Value}";
}