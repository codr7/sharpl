namespace Sharpl.Ops;

public class SetMapItem : Op
{
    public static Op Make() => new SetMapItem();
    public SetMapItem() { }
    public OpCode Code => OpCode.SetMapItem;
    public string Dump(VM vm) => $"SetMapItem";
}