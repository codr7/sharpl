namespace Sharpl.Ops;

public class CreateIter : Op
{
    public static Op Make(Register target, Loc loc) => new CreateIter(target, loc);
    public readonly Register Target;
    public readonly Loc Loc;
    public CreateIter(Register target, Loc loc)
    {
        Target = target;
        Loc = loc;
    }

    public OpCode Code => OpCode.CreateIter;
    public string Dump(VM vm) => $"CreateIter {Loc} {Target}";
}