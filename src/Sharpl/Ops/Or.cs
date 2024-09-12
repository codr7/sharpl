namespace Sharpl.Ops;

public readonly record struct Or(Label Done) : Op
{
    public static Op Make(Label done) => new Or(done);
    public override string ToString() => $"Or {Done}";
}