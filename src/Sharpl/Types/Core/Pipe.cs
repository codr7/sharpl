using System.Text;
using System.Threading.Channels;
using Sharpl.Iters.Core;

namespace Sharpl.Types.Core;

public class PipeType : Type<Channel<Value>>, IterTrait
{
    public static Channel<Value> Make() => Channel.CreateUnbounded<Value>();

    public PipeType(string name) : base(name) { }

    public override void Call(Loc loc, VM vm, Stack stack, int arity) =>
        stack.Push(Libs.Core.Pipe, Channel.CreateUnbounded<Value>());

    public override void Call(Loc loc, VM vm, Stack stack, Value target, int arity, int registerCount)
    {
        var t = target.Cast(loc, this);

        switch (arity)
        {
            case 0:
                stack.Push(Task.Run(async () => await t.Reader.ReadAsync()).Result);
                break;
            case 1:
                Task.Run(async () => await t.Writer.WriteAsync(stack.Pop()));
                break;
            default:
                throw new EvalError(loc, "Invalid arguments");
        }
    }

    public Iter CreateIter(Value target) => new PipeItems(target.Cast(this).Reader);
    public override void Dump(Value value, VM vm, StringBuilder result) => result.Append($"(Pipe {vm.GetObjectId(value.Cast(this))})");
}