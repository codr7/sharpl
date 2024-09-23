namespace Sharpl.Iters.Core;

public class Nil : Iter
{
    public static readonly Nil Instance = new Nil();
    public override Value? Next(VM vm, Loc loc) => null;
}