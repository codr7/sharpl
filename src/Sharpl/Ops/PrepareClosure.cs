namespace Sharpl.Ops;

public class PrepareClosure : Op
{
    public static Op Make(UserMethod target, Label skip) => new PrepareClosure(target, skip);
    public readonly UserMethod Target;
    public readonly Label Skip;
    public PrepareClosure(UserMethod target, Label skip)
    {
        Target = target;
        Skip = skip;
    }

    public OpCode Code => OpCode.PrepareClosure;
    public string Dump(VM vm) => $"PrepareClosure {Target} {Skip}";
}