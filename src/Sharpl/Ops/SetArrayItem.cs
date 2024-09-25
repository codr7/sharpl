namespace Sharpl.Ops;

public class SetArrayItem : Op
{
    public static Op Make(int index) => new SetArrayItem(index);
    public readonly int Index;
    public SetArrayItem(int index)
    {
        Index = index;
    }

    public OpCode Code => OpCode.SetArrayItem;
    public string Dump(VM vm) => $"SetArrayItem {Index}";
}