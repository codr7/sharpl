namespace Sharpl.Ops;

public class CreatePair : Op
{
    public static Op Make(Loc loc) => new CreatePair(loc);
    public readonly Loc Loc;
    public CreatePair(Loc loc): base(OpCode.CreatePair)
    {
        Loc = loc;
    }

    public override string Dump(VM vm) => $"CreatePair {Loc}";
}