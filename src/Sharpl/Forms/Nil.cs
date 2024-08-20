namespace Sharpl.Forms;

public class Nil : Form
{
    public Nil(Loc loc) : base(loc) { }
    public override void Emit(VM vm, Queue args) => args.PushFirst(new Literal(Loc, Value.Nil));
    public override bool Equals(Form other) => other is Nil;
    public override bool IsSplat => false;
    public override string ToString() => "_";
}