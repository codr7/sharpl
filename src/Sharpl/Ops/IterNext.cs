namespace Sharpl.Ops;

public class IterNext : Op
{
    public static Op Make(Register iter, Register value, Label done, Loc loc) =>
        new IterNext(iter, value, done, loc);

    public readonly Register Iter;
    public readonly Register Value;
    public readonly Label Done;
    public readonly Loc Loc;

    public IterNext(Register iter, Register value, Label done, Loc loc)
    {
        Iter = iter;
        Value = value;
        Done = done;
        Loc = loc;
    }

    public OpCode Code => OpCode.IterNext;
    public string Dump(VM vm) => $"IterNext {Iter} {Value} {Done} {Loc}";
}