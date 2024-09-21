using Sharpl.Iters.Core;
using System.Text;
using System.Threading.Channels;

namespace Sharpl.Types.Core;

public class PipeType : Type<Channel<Value>>, IterTrait, PollTrait
{
    public static Channel<Value> Make() => Channel.CreateUnbounded<Value>();

    public PipeType(string name) : base(name) { }

    public override void Call(VM vm, Stack stack, int arity, Loc loc) =>
        stack.Push(Libs.Core.Pipe, Channel.CreateUnbounded<Value>());

    public override void Call(VM vm, Stack stack, Value target, int arity, int registerCount, bool eval, Loc loc)
    {
        var t = target.Cast(this, loc);

        switch (arity)
        {
            case 0:
                stack.Push(Task.Run(async () => await t.Reader.ReadAsync()).Result);
                break;
            case 1:
                var v = stack.Pop();
                Task.Run(async () => await t.Writer.WriteAsync(v));
                break;
            default:
                throw new EvalError("Invalid arguments", loc);
        }
    }

    public Iter CreateIter(Value target, VM vm, Loc loc) => new PipeItems(target.Cast(this).Reader);
    public override void Dump(Value value, VM vm, StringBuilder result) => result.Append($"(Pipe {vm.GetObjectId(value.Cast(this))})");
    public Task<bool> Poll(Value target, CancellationToken ct) =>
        target.Cast(this).Reader.WaitToReadAsync(ct).AsTask();
}