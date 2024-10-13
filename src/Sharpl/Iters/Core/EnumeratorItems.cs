namespace Sharpl.Iters.Core;

public class EnumeratorItems(IEnumerator<Value> Source) : Iter
{
    public override bool Next(VM vm, Register result, Loc loc)
    {
        if (Source.MoveNext())
        {
            vm.Set(result, Source.Current);
            return true;
        }

        return false;
    }
}