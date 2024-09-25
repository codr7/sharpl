namespace Sharpl.Ops;

public class Drop : Op
{
    public static Op Make(int count) => new Drop(count);
    public readonly int Count;
    public Drop(int count): base(OpCode.Drop) {  Count = count; }
    public override string Dump(VM vm) => $"Drop {Count}";
}