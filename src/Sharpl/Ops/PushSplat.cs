namespace Sharpl.Ops;
public class PushSplat : Op
{
    public static Op Make() => new PushSplat();
    public PushSplat(): base(OpCode.PushSplat) { }
    public override string Dump(VM vm) => "PushSplat";
}