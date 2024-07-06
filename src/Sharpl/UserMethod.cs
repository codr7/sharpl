using System.Data;
using System.Runtime.Intrinsics.X86;
using System.Text;
using Sharpl.Libs;

namespace Sharpl;

public class UserMethod
{
    public readonly (string, int)[] Args;
    public readonly Dictionary<Register, (int, Value?)> Closure = new Dictionary<Register, (int, Value?)>();
    public readonly Loc Loc;
    public readonly string Name;
    public int? StartPC;

    public UserMethod(Loc loc, VM vm, string name, string[] closure, (string, int)[] args)
    {
        Loc = loc;
        Name = name;

        Closure = closure.Select<string, (Register, (int, Value?))>((id) =>
        {
#pragma warning disable CS8629
            var b = ((Value)vm.Env[id]).Cast(Core.Binding);
#pragma warning restore CS8629
            var r = vm.AllocRegister();
            vm.Env[id] = Value.Make(Core.Binding, new Register(0, r));
            return (b, (r, null));
        }).ToDictionary();

        Args = args;
    }

    public void BindArgs(VM vm, int arity, Stack stack) {
        for (var i = Args.Length - 1; i >= 0; i--)
        {
            vm.SetRegister(0, Args[i].Item2, stack.Pop());
        }

        foreach (var (s, (d, v)) in Closure)
        {
            if (v is not null)
            {
                vm.SetRegister(0, d, (Value)v);
            }
        }
    }

    public override string ToString()
    {
        var result = new StringBuilder();
        result.Append($"(Method {Name} [");

        for (var i = 0; i < Args.Length; i++)
        {
            if (i > 0)
            {
                result.Append(' ');
            }

            result.Append(Args[i].Item1);
        }

        result.Append("])");
        return result.ToString();
    }
}

