namespace Sharpl.Ops;
public class Stop : Op
{
    public static readonly Op Instance = new Stop();
    public static Op Make() => Instance;
    public Stop() { }
    public OpCode Code => OpCode.Stop;
    public string Dump(VM vm) => $"Stop";
}