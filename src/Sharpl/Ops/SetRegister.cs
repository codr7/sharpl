namespace Sharpl.Ops;

public class SetRegister : Op
{
    public static Op Make(Register target, Register value) => new SetRegister(target, value);
    public readonly Register Target;
    public readonly Register Value;
    public SetRegister(Register target, Register value)
    {
        Target = target;
        Value = value;
    }

    public OpCode Code => OpCode.SetRegister;
    public string Dump(VM vm) => $"SetRegister {Target} {Value}";
}