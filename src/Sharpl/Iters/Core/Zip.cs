namespace Sharpl.Iters.Core;

public class Zip(Iter[] Sources) : BasicIter
{
    public override Value? Next()
    {
        var vs = Sources.Select(it => it.Next()).ToArray();

#pragma warning disable CS8629
        return vs.Any(v => v is null) ? null : vs
          .Select(v => (Value)v)
          .Reverse()
          .Aggregate((a, v) => Value.Make(Libs.Core.Pair, (v, a)));
#pragma warning restore CS8629
    }
}