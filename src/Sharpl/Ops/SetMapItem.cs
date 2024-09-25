namespace Sharpl.Ops;

public class SetMapItem : Op
{
    public static Op Make() => new SetMapItem();
    public SetMapItem() : base(OpCode.SetMapItem) { }
    public override string Dump(VM vm) => $"SetMapItem";
}