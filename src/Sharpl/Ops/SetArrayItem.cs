namespace Sharpl.Ops;

public class SetArrayItem : Op
{
    public static Op Make(Register target, int index, Register value) => new SetArrayItem(target, index, value);
    public readonly Register Target;
    public readonly int Index;
    public readonly Register Value;
    public SetArrayItem(Register target, int index, Register value)
    {
        Target = target;
        Index = index;
        Value = value;
    }

    public OpCode Code => OpCode.SetArrayItem;
    public string Dump(VM vm) => $"SetArrayItem {Target} {Index} {Value}";
}