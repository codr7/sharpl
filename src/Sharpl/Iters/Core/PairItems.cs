namespace Sharpl.Iters.Core;

public class PairItems : Iter
{
    private Value? value;

    public PairItems(Value start)
    {
        value = start;
    }

    public override bool Next(VM vm, Register result, Loc loc)
    {
        if (value is Value v)
        {
            if (v.Type == Libs.Core.Pair)
            {
                var p = v.CastUnbox(Libs.Core.Pair);
                value = p.Item2;
                vm.Set(result, p.Item1);
                return true;
            }
            else
            {
                value = null;
                vm.Set(result, v);
                return true;
            }

        }

        return false;
    }
}