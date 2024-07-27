using Sharpl.Libs;

namespace Sharpl.Forms;

public class Literal : Form
{
    public readonly Value Value;

    public Literal(Loc loc, Value value) : base(loc)
    {
        Value = value;
    }

    public override void Emit(VM vm, Queue args, int quoted)
    {
        if (quoted == 0) {
            Value.Emit(Loc, vm, args);
        } else {
            vm.Emit(Ops.Push.Make(Value.Make(Core.Form, (this, quoted))));
        }
    }

    public override void EmitCall(VM vm, Queue args)
    {
        Value.EmitCall(Loc, vm, args);
    }

    public override bool Equals(Form other)
    {
        if (other is Literal l) {
            return l.Value.Equals(Value);
        }

        return false;
    }

    public override Form Expand(VM vm, int quoted)
    {
        return this; 
    }

    public override Value? GetValue(VM vm) { 
        return Value;
    }

    public override string ToString() {
        return Value.ToString();
    }
}