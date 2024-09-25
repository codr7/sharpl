namespace Sharpl.Ops;

public class PopItem : Op
{
    public static Op Make(Register target, Loc loc) => new PopItem(target, loc);
    public readonly Register Target;
    public readonly Loc Loc;
    public PopItem(Register target, Loc loc) : base(OpCode.PopItem)
    {
        Target = target;
        Loc = loc;
    }

    public override string Dump(VM vm) => $"PopItem {Loc} {Target}";
}