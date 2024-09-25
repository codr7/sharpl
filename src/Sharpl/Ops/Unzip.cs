namespace Sharpl.Ops;

public class Unzip : Op
{
    public static Op Make(Loc loc) => new Unzip(loc);
    public readonly Loc Loc;
    public Unzip(Loc loc)
    {
        Loc = loc;
    }

    public OpCode Code => OpCode.Unzip;
    public string Dump(VM vm) => $"Unzip {Loc}";
}