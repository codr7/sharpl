namespace Sharpl.Ops;
public class SetRegisterDirect : Op
{
    public static Op Make(Register target, Value value) => new SetRegisterDirect(target, value);
    public readonly Register Target;
    public readonly Value Value;
    public SetRegisterDirect(Register target, Value value)
    {
        Target = target;
        Value = value;
    }

    public OpCode Code => OpCode.SetRegisterDirect;
    public string Dump(VM vm) => $"SetRegisterDirect {Target} {Value}";
}