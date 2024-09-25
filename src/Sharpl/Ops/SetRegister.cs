namespace Sharpl.Ops;

public class SetRegister : Op
{
    public static Op Make(Register target) => new SetRegister(target);
    public readonly Register Target;
    public SetRegister(Register target): base(OpCode.SetRegister)
    {
        Target = target;
    }

    public override string Dump(VM vm) => $"SetRegister {Target}";
}