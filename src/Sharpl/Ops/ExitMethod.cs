namespace Sharpl.Ops;
public class ExitMethod : Op
{
    public static Op Instance = new ExitMethod();
    public static Op Make() => Instance;
    public ExitMethod() { }
    public OpCode Code => OpCode.ExitMethod;
    public string Dump(VM vm) => $"ExitMethod";
}