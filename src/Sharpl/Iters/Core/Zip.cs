namespace Sharpl.Iters.Core;

public class Zip(Iter[] Sources) : Iter
{
    public override bool Next(VM vm, Register result, Loc loc) => Sources.Any(it => it.Next(vm, result, loc));
}