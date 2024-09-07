namespace Sharpl.Ops;

public readonly record struct And(Label Done)
{
    public static Op Make(Label done) => new Op(Op.T.And, new And(done));
    public override string ToString() => $"And {Done}";
}