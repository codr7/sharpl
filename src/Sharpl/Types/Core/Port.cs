using System.Text;
using Sharpl.Iters.Core;

namespace Sharpl.Types.Core;

public class PortType : Type<Port>, CloseTrait, IterTrait, PollTrait
{
    public PortType(string name) : base(name) { }

    public override void Call(VM vm, Stack stack, Value target, int arity, int registerCount, Loc loc)
    {
        var t = target.Cast(this, loc);

        switch (arity)
        {
            case 0:
                    stack.Push(Task.Run(() => t.Read(vm, loc)).Result ?? Value._);
                    break;
            case 1:
                var v = stack.Pop();
                Task.Run(async () => await t.Write(v, vm, loc));
                break;
            default:
                throw new EvalError("Invalid arguments", loc);
        }
    }

    public void Close(Value target) => target.Cast(this).Close();

    public Iter CreateIter(Value target, VM vm, Loc loc) => new PortItems(target.Cast(this), vm, loc);
    public override void Dump(Value value, VM vm, StringBuilder result) => result.Append($"(Port {vm.GetObjectId(value.Cast(this))})");
    public Task<bool> Poll(Value target, CancellationToken ct) => target.Cast(this).Poll(ct);
}