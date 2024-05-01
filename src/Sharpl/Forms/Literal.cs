namespace Sharpl.Forms;

public class Literal : Form
{
    public readonly Value Value;

    public Literal(Loc loc, Value value) : base(loc)
    {
        Value = value;
    }

    public override void Emit(VM vm, Lib lib, Form.Queue args)
    {
            vm.Emit(Ops.Push.Make(Value));
    }

    public override void EmitCall(VM vm, Lib lib, Form.Queue args)
    {
        Value.EmitCall(Loc, vm, lib, args);
    }
}