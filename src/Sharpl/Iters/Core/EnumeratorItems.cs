namespace Sharpl.Iters.Core;

public class EnumeratorItems(IEnumerator<Value> Source) : BasicIter
{
    public override Value? Next() => Source.MoveNext() ? Source.Current : null;
}