namespace Sharpl.Ops;
public class Repush : Op
{
    public static Op Make(int count) => new Repush(count);
    public readonly int Count;
    public Repush(int count)
    {
        Count = count;
    }

    public OpCode Code => OpCode.Repush;
    public string Dump(VM vm) => $"Repush {Count}";
}