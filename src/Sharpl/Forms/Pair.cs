namespace Sharpl.Forms;

using System.Text;
using Sharpl.Libs;

public class Pair : Form
{
    public readonly Form Left;
    public readonly Form Right;

    public Pair(Loc loc, Form  left, Form right) : base(loc)
    {
       Left = left;
       Right = right;
    }

    public override void CollectIds(HashSet<string> result)
    {
        Left.CollectIds(result);
        Right.CollectIds(result);
    }

    public override void Emit(VM vm, Form.Queue args)
    {
        var leftArgs = new Form.Queue();
        Left.Emit(vm, leftArgs);
        Right.Emit(vm, args);
        vm.Emit(Ops.CreatePair.Make(Loc));
    }

    public override string ToString()
    {
        return $"{Left}:{Right}";
    }
}