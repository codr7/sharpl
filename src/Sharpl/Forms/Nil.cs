namespace Sharpl.Forms;

public class Nil : Form
{
    public Nil(Loc loc) : base(loc) { }
    public override void Emit(VM vm, Queue args, Register result) => vm.Emit(new Literal(Value._, Loc), result);
    public override bool Equals(Form other) => other is Nil;
    public override bool IsSplat => false;
    public override string Dump(VM vm) => "_";
}