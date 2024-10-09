namespace Sharpl.Ops;

public class Unzip : Op
{
    public static Op Make(Register target, Register? left, Register? right, Loc loc) => new Unzip(target, left, right, loc);

    public readonly Register Target;
    public readonly Register? Left;
    public readonly Register? Right;
    public readonly Loc Loc;
    public Unzip(Register target, Register? left, Register? right, Loc loc)
    {
        Loc = loc;
        Target = target;
        Left = left;
        Right = right;
    }

    public OpCode Code => OpCode.Unzip;
    public string Dump(VM vm) => $"Unzip {Target} {Left} {Right} {Loc}";
}