namespace Sharpl.Ops;

public readonly record struct SetLoadPath(String Path)
{
    public static Op Make(String path)
    {
        return new Op(Op.T.SetLoadPath, new SetLoadPath(path));
    }

    public override string ToString() {
        return $"(set-load-path {Path})";
    }
}