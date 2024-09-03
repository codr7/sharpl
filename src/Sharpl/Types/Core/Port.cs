using System.Text;
using Sharpl.Iters.Core;

namespace Sharpl.Types.Core;

public class PortType : Type<Port>, IterTrait, PollTrait
{
    public PortType(string name) : base(name) { }

    public override void Call(Loc loc, VM vm, Stack stack, Value target, int arity, int registerCount)
    {
        var t = target.Cast(loc, this);

        switch (arity)
        {
            case 0:
                stack.Push(Task.Run(t.Read).Result);
                break;
            case 1:
                var v = stack.Pop();
                Task.Run(async () => await t.Write(v));
                break;
            default:
                throw new EvalError(loc, "Invalid arguments");
        }
    }

    public Iter CreateIter(Value target) => new PortItems(target.Cast(this));
    public override void Dump(Value value, VM vm, StringBuilder result) => result.Append($"(Port {vm.GetObjectId(value.Cast(this))})");
    public Task<bool> Poll(Value target, CancellationToken ct) => target.Cast(this).Poll(ct);
}