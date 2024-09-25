namespace Sharpl.Ops;

public class CreatePair : Op
{
    public static Op Make(Loc loc) => new CreatePair(loc);
    public readonly Loc Loc;
    public CreatePair(Loc loc)
    {
        Loc = loc;
    }

    public OpCode Code => OpCode.CreatePair;
    public string Dump(VM vm) => $"CreatePair {Loc}";
}