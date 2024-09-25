namespace Sharpl.Ops;

public class SetRegister : Op
{
    public static Op Make(Register target) => new SetRegister(target);
    public readonly Register Target;
    public SetRegister(Register target)
    {
        Target = target;
    }

    public OpCode Code => OpCode.SetRegister;
    public string Dump(VM vm) => $"SetRegister {Target}";
}