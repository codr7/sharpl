namespace Sharpl.Ops;

public readonly record struct Or(Label Done)
{
    public static Op Make(Label done) => new Op(Op.T.Or, new Or(done));
    public override string ToString() => $"Or {Done}";
}