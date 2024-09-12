namespace Sharpl.Ops;

public readonly record struct And(Label Done) : Op
{
    public static Op Make(Label done) => new And(done);
    public override string ToString() => $"And {Done}";
}