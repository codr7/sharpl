namespace Sharpl.Ops;

public class OpenInputStream : Op
{
    public static Op Make(string path, Register result, Loc loc) =>
        new OpenInputStream(path, result, loc);

    public readonly string Path;
    public readonly Register Result;
    public readonly Loc Loc;

    public OpenInputStream(string path, Register result, Loc loc)
    {
        Path = path;
        Result = result;
        Loc = loc;
    }

    public OpCode Code => OpCode.OpenInputStream;
    public  string Dump(VM vm) => $"OpenInputStream {Path} {Result} {Loc}";
}