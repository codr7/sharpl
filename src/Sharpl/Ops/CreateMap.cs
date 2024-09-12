namespace Sharpl.Ops;

public readonly record struct CreateMap(int Length) : Op
{
    public static Op Make(int length) => new CreateMap(length);
    public override string ToString() => $"CreateMap {Length}";
}