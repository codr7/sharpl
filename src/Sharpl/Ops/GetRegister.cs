namespace Sharpl.Ops;

public class GetRegister : Op
{
    public static Op Make(Register target) => new GetRegister(target);
    public readonly Register Target;
    public GetRegister(Register target)
    {
        Target = target;
    }

    public OpCode Code => OpCode.GetRegister;
    public string Dump(VM vm) => $"GetRegister {Target}";
}