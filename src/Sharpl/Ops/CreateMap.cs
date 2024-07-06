namespace Sharpl.Ops;

public readonly record struct CreateMap(int Length)
{
    public static Op Make(int length)
    {
        return new Op(Op.T.CreateMap, new CreateMap(length));
    }

    public override string ToString() {
        return $"CreateMap {Length}";
    }    
}