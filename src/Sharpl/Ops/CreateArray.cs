namespace Sharpl.Ops;

public class CreateArray : Op
{
    public static Op Make(int length, Register target) => new CreateArray(length, target);
    public readonly int Length;
    public readonly Register Target;
    public CreateArray(int length, Register target)
    {
        Length = length;
        Target = target;
    }

    public OpCode Code => OpCode.CreateArray;
    public string Dump(VM vm) => $"CreateArray {Length} {Target}";
}