namespace Sharpl.Iters.Core;

public class Nil : Iter
{
    public static readonly Nil Instance = new Nil();
    public override bool Next(VM vm, Register result, Loc loc) => false;
}