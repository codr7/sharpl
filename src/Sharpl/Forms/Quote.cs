namespace Sharpl.Forms;

public class Quote : Form
{
    public readonly Form Target;


    public Quote(Loc loc, Form target) : base(loc)
    {
        Target = target;
    }

    public override void CollectIds(HashSet<string> result)
    {
        Target.CollectIds(result);
    }

    public override void Emit(VM vm, Form.Queue args, int quoted)
    {
        Target.Emit(vm, args, quoted + 1);
    }

    public override string ToString()
    {
        return $"'{Target}";
    }
}