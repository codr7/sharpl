namespace Sharpl.Operations
{
    public readonly record struct Push(Value Value)
    {
        public static Operation Make(Value value)
        {
            return new Operation(Operation.T.Push, new Push(value));
        }
    }
}