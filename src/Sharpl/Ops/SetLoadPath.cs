namespace Sharpl.Ops;

public readonly record struct SetLoadPath(string Path)
{
    public static Op Make(string path)
    {
        return new Op(Op.T.SetLoadPath, new SetLoadPath(path));
    }

    public override string ToString() {
        return $"(set-load-path {Path})";
    }
}