using System.Runtime.Intrinsics.X86;
using System.Text;

namespace Sharpl.Types.Core;

public class FixType(string name, AnyType[] parents) :
    ComparableType<ulong>(name, parents),
    NumericTrait,
    RangeTrait
{
    public override bool Bool(Value value) => Fix.Val(value.CastUnbox(this)) != 0;

    public override void Call(VM vm, int arity, Register result, Loc loc)
    {
        if (arity == 0) { throw new EvalError("Wrong number of args", loc);  }
        var v = vm.GetRegister(0, 0).CastUnbox(Libs.Core.Int, loc);
        var e = (arity == 2) ? vm.GetRegister(0, 1).CastUnbox(Libs.Core.Int, loc) : 1;
        vm.Set(result, Value.Make(Libs.Core.Fix, Fix.Make((byte)e, v)));
    }

    public Iter CreateRange(Value min, Value max, Value stride, Loc loc)
    {
        ulong? minVal = min.TryCastUnbox(this);
        ulong? maxVal = max.TryCastUnbox(this);
        if (stride.Type == Libs.Core.Nil) { throw new EvalError("Missing stride", loc); }
        ulong strideVal = stride.CastUnbox(this);
        return new Iters.Core.FixRange(minVal ?? Fix.Make(1, 0), maxVal, strideVal);
    }

    public override void Dump(VM vm, Value value, StringBuilder result) =>
        result.Append(Fix.ToString(value.CastUnbox(this)));

    public void Add(VM vm, int arity, Register result, Loc loc)
    {
        if (arity == 0) { vm.Set(result, Value.Make(this, Fix.Make(1, 0))); }
        var res = vm.GetRegister(0, 0).CastUnbox(this, loc);
        for (var i = 1; i < arity; i++) { res = Fix.Add(res, vm.GetRegister(0, i).CastUnbox(this, loc)); }
        vm.Set(result, Value.Make(this, res));
    }

    public void Divide(VM vm, int arity, Register result, Loc loc)
    {
        if (arity == 0) { vm.Set(result, Value.Make(this, Fix.Make(1, 0))); }
        var res = vm.GetRegister(0, 0).CastUnbox(this, loc);
        for (var i = 1; i < arity; i++) { res = Fix.Divide(res, vm.GetRegister(0, i).CastUnbox(this, loc)); }
        vm.Set(result, Value.Make(this, res));
    }

    public override bool Equals(Value left, Value right) => Fix.Equals(left.CastUnbox(this), right.CastUnbox(this));

    public void Multiply(VM vm, int arity, Register result, Loc loc)
    {
        if (arity == 0) { vm.Set(result, Value.Make(this, Fix.Make(1, 0))); }
        var res = vm.GetRegister(0, 0).CastUnbox(this, loc);
        for (var i = 1; i < arity; i++) { res = Fix.Multiply(res, vm.GetRegister(0, i).CastUnbox(this, loc)); }
        vm.Set(result, Value.Make(this, res));
    }

    public void Subtract(VM vm, int arity, Register result, Loc loc)
    {
        var res = Fix.Make(1, 0);

        if (arity > 0)
        {
            if (arity == 1) { res = Fix.Negate(vm.GetRegister(0, 0).CastUnbox(this, loc)); }
            else
            {
                res = vm.GetRegister(0, 0).CastUnbox(this, loc);
                for (var i = 1; i < arity; i++) { res = Fix.Subtract(res, vm.GetRegister(0, 1).CastUnbox(this, loc)); }
            }
        }

        vm.Set(result, Value.Make(this, res));
    }

    public override string ToJson(Value value, Loc loc) => Fix.ToString(value.CastUnbox(this), true);
}