namespace Sharpl.Ops;

public class Or : Op
{
    public static Op Make(Register target, Label done) => new Or(target, done);

    public readonly Register Target;
    public readonly Label Done;
    public Or(Register target, Label done)
    {
        Target = target;
        Done = done;
    }

    public OpCode Code => OpCode.Or;
    public string Dump(VM vm) => $"Or {Target} {Done}";
}