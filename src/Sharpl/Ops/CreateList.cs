namespace Sharpl.Ops;

public class CreateList : Op
{
    public static Op Make(Register target) => new CreateList(target);
    public readonly Register Target;
    public CreateList(Register target)
    {
        Target = target;
    }

    public OpCode Code => OpCode.CreateList;
    public string Dump(VM vm) => $"CreateList {Target}";
}