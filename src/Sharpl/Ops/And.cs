namespace Sharpl.Ops;

public class And: Op
{
    public static Op Make(Label done) => new And(done);
    public readonly Label Done;

    public And(Label done): base(OpCode.And)
    {
        Done = done;
    }

    public override string Dump(VM vm) => $"And {Done}";
}