namespace Sharpl.Ops;

public class SetLoadPath : Op
{
    public static Op Make(string path) => new SetLoadPath(path);
    public readonly string Path;
    public SetLoadPath(string path)
    {
        Path = path;
    }

    public OpCode Code => OpCode.SetLoadPath;
    public string Dump(VM vm) => $"SetLoadPath {Path}";
}