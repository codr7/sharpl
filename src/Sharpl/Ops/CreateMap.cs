namespace Sharpl.Ops;

public class CreateMap : Op
{
    public static Op Make(int length) => new CreateMap(length);
    public readonly int Length;
    public CreateMap(int length) : base(OpCode.CreateMap)
    {
        Length = length;
    }

    public override string Dump(VM vm) => $"CreateMap {Length}";
}