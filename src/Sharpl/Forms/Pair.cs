using Sharpl.Libs;

namespace Sharpl.Forms;

public class Pair : Form
{
    public readonly Form Left;
    public readonly Form Right;

    public Pair(Form left, Form right, Loc loc) : base(loc)
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
        vm.Emit(Left);
        vm.Emit(Right);
        vm.Emit(Ops.CreatePair.Make(Loc));
    }

    public override bool Equals(Form other) =>
        (other is And f) ? f.Left.Equals(Left) && f.Right.Equals(Right) : false;

    public override bool Expand(VM vm, Queue args) {
        var result = false;
        if (Left.Expand(vm, args)) { result = true; }
        var l = args.PopLast();
        if (Right.Expand(vm, args)) { result = true; }
        var r = args.PopLast();
        args.Push(new Pair(l, r, Loc));
        return result;
    }

    public override Value? GetValue(VM vm) =>
        (Left is Literal ll && Right is Literal rr) ? Value.Make(Core.Pair, (ll.Value.Copy(), rr.Value.Copy())) : null;

    public override Form Quote(VM vm, Loc loc) => 
        new Pair(Left.Quote(vm, loc), Right.Quote(vm, loc), loc);

    public override string Dump(VM vm) => $"{Left.Dump(vm)}:{Right.Dump(vm)}";

    public override Form Unquote(VM vm, Loc loc) => 
        new Pair(Left.Unquote(vm, loc), Right.Unquote(vm, loc), loc);
}