namespace Sharpl.Forms;

public class Literal : Form
{
    public readonly Value Value;

    public Literal(Value value, Loc loc) : base(loc)
    {
        Value = value;
    }

    public override void Emit(VM vm, Queue args) => Value.Emit(Loc, vm, args);
    public override void EmitCall(VM vm, Queue args) => Value.EmitCall(Loc, vm, args);
    public override bool Equals(Form other) => (other is Literal l) && l.Value.Equals(Value);
    public override Value? GetValue(VM vm) => Value.Copy();
    public override string Dump(VM vm) => Value.Dump(vm);
    public override Form Unquote(Loc loc, VM vm) => Value.Unquote(loc, vm);
}