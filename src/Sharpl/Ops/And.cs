namespace Sharpl.Ops;

public class And: Op
{
    public static Op Make(Label done) => new And(done);
    public readonly Label Done;

    public And(Label done)
    {
        Done = done;
    }

    public OpCode Code => OpCode.And;
    public string Dump(VM vm) => $"And {Done}";
}