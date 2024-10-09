namespace Sharpl.Ops;

public class And: Op
{
    public static Op Make(Register target, Label done) => new And(target, done);
    
    public readonly Register Target;
    public readonly Label Done;

    public And(Label done, Register target)
    {
        Done = done;
        this.Target = target;

    }

    public OpCode Code => OpCode.And;
    public string Dump(VM vm) => $"And {Target} {Done}";
}