namespace Sharpl.Ops;

public class Or : Op
{
    public static Op Make(Label done) => new Or(done);
    public readonly Label Done;
    public Or(Label done)
    {
        Done = done;
    }

    public OpCode Code => OpCode.Or;
    public string Dump(VM vm) => $"Or {Done}";
}