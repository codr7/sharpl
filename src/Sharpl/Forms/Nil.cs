namespace Sharpl.Forms;

public class Nil : Form
{
    public Nil(Loc loc) : base(loc) { }

    public override void Emit(VM vm, Form.Queue args, int quoted) { }

    public override bool Equals(Form other)
    {
        return other is Nil;
    }

    public override Form Expand(VM vm, int quoted)
    {
        return this;
    }

    public override bool IsSplat => true;

    public override string ToString()
    {
        return "_";
    }
}