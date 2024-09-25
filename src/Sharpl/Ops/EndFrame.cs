namespace Sharpl.Ops;

public class EndFrame : Op
{
    public static Op Make() => new EndFrame();
    public EndFrame() { }
    public OpCode Code => OpCode.EndFrame;
    public string Dump(VM vm) => "EndFrame";
}