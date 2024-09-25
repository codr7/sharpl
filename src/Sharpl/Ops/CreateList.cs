namespace Sharpl.Ops;

public class CreateList : Op
{
    public static Op Make(Register target) => new CreateList(target);
    public readonly Register Target;
    public CreateList(Register target) : base(OpCode.CreateList)
    {
        Target = target;
    }

    public override string Dump(VM vm) => $"CreateList {Target}";
}