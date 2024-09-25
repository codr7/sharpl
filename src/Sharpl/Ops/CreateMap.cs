namespace Sharpl.Ops;

public class CreateMap : Op
{
    public static Op Make(int length) => new CreateMap(length);
    public readonly int Length;

    public CreateMap(int length)
    {
        Length = length;
    }

    public OpCode Code => OpCode.CreateMap;
    public string Dump(VM vm) => $"CreateMap {Length}";
}