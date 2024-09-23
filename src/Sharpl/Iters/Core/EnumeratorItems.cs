namespace Sharpl.Iters.Core;

public class EnumeratorItems(IEnumerator<Value> Source) : Iter
{
    public override Value? Next(VM vm, Loc loc) => Source.MoveNext() ? Source.Current : null;
}