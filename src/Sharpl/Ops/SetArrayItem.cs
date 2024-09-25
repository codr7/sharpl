namespace Sharpl.Ops;

public class SetArrayItem : Op
{
    public static Op Make(int index) => new SetArrayItem(index);
    public readonly int Index;
    public SetArrayItem(int index): base(OpCode.SetArrayItem)
    {
        Index = index;
    }

    public override string Dump(VM vm) => $"SetArrayItem {Index}";
}