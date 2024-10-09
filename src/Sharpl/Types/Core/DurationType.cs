using System.Text;

namespace Sharpl.Types.Core;

public class DurationType(string name, AnyType[] parents) :
    ComparableType<Duration>(name, parents), NumericTrait
{
    public static readonly Duration ZERO = new Duration(0, TimeSpan.FromTicks(0));

    public void Add(VM vm, int arity, Register result, Loc loc)
    {
        var res = new Duration(0, TimeSpan.FromTicks(0));
        for (var i = 0; i < arity; i++) { res = vm.GetRegister(0, i).CastUnbox(this, loc).Add(res); }
        vm.Set(result, Value.Make(this, res));
    }

    public override bool Bool(Value value) => value.CastUnbox(this).CompareTo(ZERO) > 0;

    public void Divide(VM vm, int arity, Register result, Loc loc)
    {
        var res = vm.GetRegister(0, 0).CastUnbox(this, loc);
        for (var i = 1; i < arity; i++) { res = res.Divide(vm.GetRegister(0, i).CastUnbox(Libs.Core.Int, loc)); }
        vm.Set(result, Value.Make(this, res));
    }

    public void Multiply(VM vm, int arity, Register result, Loc loc)
    {
        var res = vm.GetRegister(0, 0).CastUnbox(this, loc);
        for (var i = 1;i < arity; i++) { res = res.Multiply(vm.GetRegister(0, i).CastUnbox(Libs.Core.Int, loc)); }
        vm.Set(result, Value.Make(this, res));
    }

    public void Subtract(VM vm, int arity, Register result, Loc loc)
    {
        var res = new Duration(0, TimeSpan.FromTicks(0));

        if (arity > 0)
        {
            if (arity == 1) { res = res.Subtract(vm.GetRegister(0, 0).CastUnbox(this, loc)); }
            else
            {
                res = vm.GetRegister(0, 0).CastUnbox(this, loc);
                for (var i=1; i < arity; i++) { res = res.Subtract(vm.GetRegister(0, i).CastUnbox(this, loc)); }
            }
        }

        vm.Set(result, Value.Make(this, res));
    }

    public override void Say(VM vm, Value value, StringBuilder result) => result.Append(value.CastUnbox(this));
}