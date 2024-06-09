namespace Sharpl.Ops;

public readonly record struct CreateArray(int Length)
{
    public static Op Make(int length)
    {
        return new Op(Op.T.CreateArray, new CreateArray(length));
    }

    public override string ToString() {
        return $"(create-array {Length})";
    }    
}