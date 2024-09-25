namespace Sharpl.Ops;

public class GetRegister : Op
{
    public static Op Make(Register target) => new GetRegister(target);
    public readonly Register Target;
    public GetRegister(Register target): base(OpCode.GetRegister)
    {
        Target = target;
    }

    public override string Dump(VM vm) => $"GetRegister {Target}";
}