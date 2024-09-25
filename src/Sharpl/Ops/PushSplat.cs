namespace Sharpl.Ops;
public class PushSplat : Op
{
    public static Op Make() => new PushSplat();
    public PushSplat() { }
    public OpCode Code => OpCode.PushSplat;
    public string Dump(VM vm) => "PushSplat";
}