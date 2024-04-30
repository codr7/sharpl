namespace Sharpl.Ops
{
    public readonly record struct Stop()
    {
        public static Op instance = new Op(Op.T.Stop, new Stop());

        public static Op Make()
        {
            return instance;
        }
    }
}