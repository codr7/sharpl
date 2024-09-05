using Sharpl.Libs;

namespace Sharpl.Forms;

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
        args.Push(new Pair(Loc, l, r));
        return result;
    }

    public override Value? GetValue(VM vm) =>
        (Left is Literal ll && Right is Literal rr) ? Value.Make(Core.Pair, (ll.Value, rr.Value)) : null;

    public override Form Quote(Loc loc, VM vm) => 
        new Pair(loc, Left.Quote(loc, vm), Right.Quote(loc, vm));

    public override string Dump(VM vm) => $"{Left.Dump(vm)}:{Right.Dump(vm)}";

    public override Form Unquote(Loc loc, VM vm) => 
        new Pair(loc, Left.Unquote(loc, vm), Right.Unquote(loc, vm));
}