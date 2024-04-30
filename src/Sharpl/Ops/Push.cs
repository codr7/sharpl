namespace Sharpl.Ops
{
    public readonly record struct Push(Value Value)
    {
        public static Op Make(Value value)
        {
            return new Op(Op.T.Push, new Push(value));
        }
    }
}