using Sharpl.Iters.Core;
using System.Text;

namespace Sharpl.Types.Core;

public class PortType(string name, AnyType[] parents) : Type<Port>(name, parents), CloseTrait, IterTrait, PollTrait
{
    public override void Call(VM vm, Value target, int arity, int registerCount, bool eval, Register result, Loc loc)
    {
        var t = target.Cast(this, loc);

        switch (arity)
        {
            case 0:
                vm.Set(result, Task.Run(async () => await t.Read(vm, loc)).Result ?? Value._);
                break;
            case 1:
                var v = vm.GetRegister(0, 0);
                Task.Run(async () => await t.Write(v, vm, loc));
                break;
            default:
                throw new EvalError("Invalid arguments", loc);
        }
    }

    public void Close(Value target) => target.Cast(this).Close();

    public Iter CreateIter(Value target, VM vm, Loc loc) => new PortItems(target.Cast(this));
    
    public override void Dump(VM vm, Value value, StringBuilder result) => 
        result.Append($"(Port {vm.GetObjectId(value.Cast(this))})");
    
    public Task<bool> Poll(Value target, CancellationToken ct) => target.Cast(this).Poll(ct);
}