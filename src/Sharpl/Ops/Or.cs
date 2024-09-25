namespace Sharpl.Ops;

public class Or : Op
{
    public static Op Make(Label done) => new Or(done);
    public readonly Label Done;
    public Or(Label done) : base(OpCode.Or)
    {
        Done = done;
    }

    public override string Dump(VM vm) => $"Or {Done}";
}