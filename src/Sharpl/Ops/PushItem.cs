namespace Sharpl.Ops;

public class PushItem : Op
{
    public static Op Make(Register target, Register value, Loc loc) => new PushItem(target, value, loc);
    public readonly Register Target;
    public readonly Register Value;
    public readonly Loc Loc;
    public PushItem(Register target, Register value, Loc loc)
    {
        Target = target;
        Value = value;
        Loc = loc;
    }

    public OpCode Code => OpCode.PushItem;
    public string Dump(VM vm) => $"PushItem {Target} {Value} {Loc}";
}