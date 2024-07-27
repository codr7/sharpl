namespace Sharpl.Forms;

using System.Text;
using Sharpl.Libs;

public class Pair : Form
{
    public readonly Form Left;
    public readonly Form Right;

    public Pair(Loc loc, Form left, Form right) : base(loc)
    {
        Left = left;
        Right = right;
    }

    public override void CollectIds(HashSet<string> result)
    {
        Left.CollectIds(result);
        Right.CollectIds(result);
    }

    public override void Emit(VM vm, Form.Queue args, int quoted)
    {
        var leftArgs = new Form.Queue();
        Left.Emit(vm, leftArgs);
        Right.Emit(vm, args);
        vm.Emit(Ops.CreatePair.Make(Loc));
    }

    public override bool Equals(Form other)
    {
        if (other is And f) { return f.Left.Equals(Left) && f.Right.Equals(Right); }
        return false;
    }

    public override Form Expand(VM vm, int quoted)
    {
        return new Pair(Loc, Left.Expand(vm, quoted), Right.Expand(vm, quoted));
    }

    public override string ToString()
    {
        return $"{Left}:{Right}";
    }
}