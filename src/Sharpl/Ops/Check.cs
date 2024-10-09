namespace Sharpl.Ops;

public class Check : Op
{
    public static Op Make(Register actual, Register expected, Loc loc) => 
        new Check(actual, expected, loc);

    public readonly Register Actual;
    public readonly Register Expected;
    public readonly Register Result;
    public readonly Loc Loc;

    public Check(Register actual, Register expected, Loc loc)
    {
        Actual = actual;
        Expected = expected;
        Loc = loc;
    }

    public OpCode Code => OpCode.Check;
    public string Dump(VM vm) => $"Check {Actual} {Expected} {Loc}";
}