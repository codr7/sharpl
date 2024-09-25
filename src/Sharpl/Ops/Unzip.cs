namespace Sharpl.Ops;

public class Unzip : Op
{
    public static Op Make(Loc loc) => new Unzip(loc);
    public readonly Loc Loc;
    public Unzip(Loc loc) : base(OpCode.Unzip)
    {
        Loc = loc;
    }

    public override string Dump(VM vm) => $"Unzip {Loc}";
}