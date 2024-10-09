namespace Sharpl.Ops;

public class CreateMap : Op
{
    public static Op Make(int length, Register target) => new CreateMap(length, target);
    public readonly int Length;
    public readonly Register Target;

    public CreateMap(int length, Register target)
    {
        Length = length;
        Target = target;
    }

    public OpCode Code => OpCode.CreateMap;
    public string Dump(VM vm) => $"CreateMap {Length} {Target}";
}