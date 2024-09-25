namespace Sharpl.Ops;

public class CreateArray : Op
{
    public static Op Make(int length) => new CreateArray(length);
    public readonly int Length;
    public CreateArray(int length)
    {
        Length = length;
    }

    public OpCode Code => OpCode.CreateArray;
    public string Dump(VM vm) => $"CreateArray {Length}";
}