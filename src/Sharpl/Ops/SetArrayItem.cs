namespace Sharpl.Ops;

public readonly record struct SetArrayItem(int Index) : Op
{
    public static Op Make(int index) => new SetArrayItem(index);
    public override string ToString() => $"SetArrayItem {Index}";
}