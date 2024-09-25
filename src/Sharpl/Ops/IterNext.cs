namespace Sharpl.Ops;

public class IterNext : Op
{
    public static Op Make(Register iter, Label done, bool push, Loc loc) =>
        new IterNext(iter, done, push, loc);

    public readonly Register Iter;
    public readonly Label Done;
    public readonly bool Push;
    public readonly Loc Loc;

    public IterNext(Register iter, Label done, bool push, Loc loc): base(OpCode.IterNext)
    {
        Iter = iter;
        Done = done;
        Push = push;
        Loc = loc;
    }

    public override string Dump(VM vm) => $"IterNext {Loc} {Iter} {Done} {Push}";
}