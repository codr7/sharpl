namespace Sharpl.Ops;

public class Goto : Op
{
    public static Op Make(Label target) => new Goto(target);
    public readonly Label Target;
    public Goto(Label target): base(OpCode.Goto)
    {
        Target = target;
    }

    public override string Dump(VM vm) => $"Goto {Target}";
}