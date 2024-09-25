namespace Sharpl.Ops;

public class PushItem : Op
{
    public static Op Make(Register target, Loc loc) => new PushItem(target, loc);
    public readonly Register Target;
    public readonly Loc Loc;
    public PushItem(Register target, Loc loc)
    {
        Target = target;
        Loc = loc;
    }

    public OpCode Code => OpCode.PushItem;
    public string Dump(VM vm) => $"PushItem {Loc} {Target}";
}