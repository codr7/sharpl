namespace Sharpl.Ops;

public class EndFrame : Op
{
    public static Op Make() => new EndFrame();
    public EndFrame(): base(OpCode.EndFrame) { }
    public override string Dump(VM vm) => "EndFrame";
}