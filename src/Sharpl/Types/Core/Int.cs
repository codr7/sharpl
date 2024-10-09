namespace Sharpl.Types.Core;

public class IntType(string name, AnyType[] parents) :
    ComparableType<int>(name, parents), NumericTrait, RangeTrait
{
    public override bool Bool(Value value) => value.CastUnbox(this) != 0;

    public Iter CreateRange(Value min, Value max, Value stride, Loc loc)
    {
        int minVal = (min.Type == Libs.Core.Nil) ? 0 : min.CastUnbox(this, loc);
        int? maxVal = (max.Type == Libs.Core.Nil) ? null : max.CastUnbox(this, loc);
        int strideVal = (stride.Type == Libs.Core.Nil) ? ((maxVal is int mv && maxVal < minVal) ? -1 : 1) : stride.CastUnbox(this, loc);
        return new Iters.Core.IntRange(minVal, maxVal, strideVal);
    }

    public void Add(VM vm, int arity, Register result, Loc loc)
    {
        var res = 0;
        for (var i = 0; i < arity; i++) { res += vm.GetRegister(0, i).CastUnbox(this, loc); }
        vm.Set(result, Value.Make(this, res));
    }

    public override void Call(VM vm, Value target, int arity, int registerCount, bool eval, Register result, Loc loc)
    {
        switch (arity)
        {
            case 1:
                var nt = vm.GetRegister(0, 0);
                vm.SetRegister(0, 0, target);
                nt.Call(vm, 1, registerCount, eval, result, loc);
                break;
            default:
                base.Call(vm, target, arity, registerCount, eval, result, loc);
                break;
        }
    }

    public void Divide(VM vm, int arity, Register result, Loc loc)
    {
        var res = vm.GetRegister(0, 0).CastUnbox(this, loc);
        for (var i = 1; i < arity; i++) { res /= vm.GetRegister(0, i).CastUnbox(this, loc); }
        vm.Set(result, Value.Make(this, res));
    }

    public void Multiply(VM vm, int arity, Register result, Loc loc)
    {
        var res = vm.GetRegister(0, 0).CastUnbox(this, loc);

        for (var i = 1; i < arity; i++)
        {
            res *= vm.GetRegister(0, i).CastUnbox(this, loc);
            arity--;
        }

        vm.Set(result, Value.Make(this, res));
    }

    public void Subtract(VM vm, int arity, Register result, Loc loc)
    {
        var res = 0;

        if (arity > 0)
        {
            if (arity == 1) { res = -vm.GetRegister(0, 0).CastUnbox(this, loc); }
            else
            {
                res = vm.GetRegister(0, 0).CastUnbox(this, loc);
                for (var i = 1; i < arity; i++) { res -= vm.GetRegister(0, i).CastUnbox(this, loc); }
            }
        }

        vm.Set(result, Value.Make(this, res));
    }

    public override string ToJson(Value value, Loc loc) => $"{value.CastUnbox(this)}";
}