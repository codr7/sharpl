namespace Sharpl.Iters.Core;

public class EnumeratorItems : BasicIter
{
    public readonly IEnumerator<Value> Source;

    public EnumeratorItems(IEnumerator<Value> source)
    {
        Source = source;
    }

    public override Value? Next() => 
        Source.MoveNext() ? Source.Current : null;
}