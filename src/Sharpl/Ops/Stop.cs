namespace Sharpl.Ops;
public class Stop : Op
{
    public static readonly Op Instance = new Stop();
    public static Op Make() => Instance;
    public Stop(): base(OpCode.Stop) { }
    public override string Dump(VM vm) => $"Stop";
}