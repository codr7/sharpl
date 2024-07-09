namespace Sharpl.Forms;

public class Any : Form
{
    public Any(Loc loc) : base(loc) { }

    public override void Emit(VM vm, Form.Queue args, int quoted) { }

    public override bool IsSplat => true;

    public override string ToString()
    {
        return "?";
    }
}