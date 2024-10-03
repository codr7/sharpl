using System.Xml.Linq;

namespace Sharpl.Types.Core;

public class BindingType(string name, AnyType[] parents) : Type<Register>(name, parents)
{
    public override void EmitCall(VM vm, Value target, Form.Queue args, Loc loc)
    {
        var arity = args.Count;
        var splat = args.IsSplat;
        args.Emit(vm);
        var v = target.CastUnbox(this);
        vm.Emit(Ops.CallRegister.Make(v, arity, splat, vm.NextRegisterIndex, loc));
    }

    public override void Emit(VM vm, Value target, Form.Queue args, Loc loc) =>
        vm.Emit(Ops.GetRegister.Make(target.CastUnbox(this)));

    public override Form Unquote(VM vm, Value value, Loc loc) =>
        new Forms.Binding(value.CastUnbox(this), loc);
}
