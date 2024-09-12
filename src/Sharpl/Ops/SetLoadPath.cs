namespace Sharpl.Ops;

public readonly record struct SetLoadPath(string Path) : Op
{
    public static Op Make(string path) => new SetLoadPath(path);
    public override string ToString() => $"SetLoadPath {Path}";
}