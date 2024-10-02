namespace Sharpl.Ops;

public class EndFrame : Op
{
    public readonly Loc Loc;
    public static Op Make(Loc loc) => new EndFrame(loc);
    public EndFrame(Loc loc) { Loc = loc;  }
    public OpCode Code => OpCode.EndFrame;
    public string Dump(VM vm) => "EndFrame";
}