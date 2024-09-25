namespace Sharpl.Ops;
public class Repush : Op
{
    public static Op Make(int count) => new Repush(count);
    public readonly int Count;
    public Repush(int count): base(OpCode.Repush)
    {
        Count = count;
    }

    public override string Dump(VM vm) => $"Repush {Count}";
}