namespace Sharpl.Ops;

public class PopItem : Op
{
    public static Op Make(Register target, Register result, Loc loc) => new PopItem(target, result, loc);
    public readonly Register Target;
    public readonly Register Result;
    public readonly Loc Loc;
    public PopItem(Register target, Register result, Loc loc)
    {
        Target = target;
        Result = result;
        Loc = loc;
    }

    public OpCode Code => OpCode.PopItem;
    public string Dump(VM vm) => $"PopItem {Target} {Result} {Loc}";
}