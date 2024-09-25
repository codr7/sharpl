namespace Sharpl.Ops;

public class SetLoadPath : Op
{
    public static Op Make(string path) => new SetLoadPath(path);
    public readonly string Path;
    public SetLoadPath(string path): base(OpCode.SetLoadPath)
    {
        Path = path;
    }

    public override string Dump(VM vm) => $"SetLoadPath {Path}";
}