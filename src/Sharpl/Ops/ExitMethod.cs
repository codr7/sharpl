namespace Sharpl.Ops;
public class ExitMethod : Op
{
    public static Op Instance = new ExitMethod();
    public static Op Make() => Instance;
    public ExitMethod(): base(OpCode.ExitMethod) { }
    public override string Dump(VM vm) => $"ExitMethod";
}