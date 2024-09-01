namespace Sharpl.Iters.Core;

public class Nil : BasicIter
{
    public static readonly Nil Instance = new Nil();
    public override Value? Next() => null;
}