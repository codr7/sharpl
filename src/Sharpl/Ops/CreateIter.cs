namespace Sharpl.Ops;

public class CreateIter : Op
{
    public static Op Make(Register target, Loc loc) => new CreateIter(target, loc);
    public readonly Register Target;
    public readonly Loc Loc;
    public CreateIter(Register target, Loc loc) : base(OpCode.CreateIter)
    {
        Target = target;
        Loc = loc;
    }
    public override string Dump(VM vm) => $"CreateIter {Loc} {Target}";
}