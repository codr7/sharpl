namespace Sharpl.Ops;

public class SetMapItem : Op
{
    public static Op Make(Register target, Register key, Register value) => new SetMapItem(target, key, value);

    public readonly Register Target;
    public readonly Register Key;
    public readonly Register Value;

    public SetMapItem(Register target, Register key, Register value) {
        Target = target; 
        Key = key; 
        Value = value;  
    }

    public OpCode Code => OpCode.SetMapItem;
    public string Dump(VM vm) => $"SetMapItem";
}