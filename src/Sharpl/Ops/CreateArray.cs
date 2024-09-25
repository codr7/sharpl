namespace Sharpl.Ops;

public class CreateArray : Op
{
    public static Op Make(int length) => new CreateArray(length);
    public readonly int Length;
    public CreateArray(int length) : base(OpCode.CreateArray)
    {
        Length = length;
    }


    public override string Dump(VM vm) => $"CreateArray {Length}";
}