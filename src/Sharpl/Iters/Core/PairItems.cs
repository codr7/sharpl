namespace Sharpl.Iters.Core;

public class PairItems : BasicIter
{
    private Value? value;

    public PairItems(Value start)
    {
        value = start;
    }

    public override Value? Next()
    {
        if (value is Value v)
        {
            if (v.Type == Libs.Core.Pair)
            {
                var p = v.CastUnbox(Libs.Core.Pair);
                value = p.Item2;
                return p.Item1;
            }
            else
            {
                value = null;
                return v;
            }

        }

        return null;
    }
}