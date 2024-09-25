namespace Sharpl.Ops;

public class Goto : Op
{
    public static Op Make(Label target) => new Goto(target);
    public readonly Label Target;
    public Goto(Label target)
    {
        Target = target;
    }

    public OpCode Code => OpCode.Goto;
    public string Dump(VM vm) => $"Goto {Target}";
}