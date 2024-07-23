using System.Data;
using System.Text;
using Sharpl.Libs;

namespace Sharpl;

public class UserMethod
{
    public readonly (string, int)[] Args;
    public readonly (string, int, Register)[] Closure;
    public readonly Dictionary<int, Value> ClosureValues = new Dictionary<int, Value>();
    public readonly Loc Loc;
    public readonly string Name;
    public int? StartPC;
    public readonly bool Vararg;

    public UserMethod(Loc loc, VM vm, string name, string[] ids, (string, int)[] args, bool vararg)
    {
        Loc = loc;
        Name = name;

        Closure = ids.AsEnumerable().Select<string, (string, int, Register)>(id =>
        {
#pragma warning disable CS8629
            var b = ((Value)vm.Env[id]).Cast(Core.Binding);
#pragma warning restore CS8629
            var r = vm.AllocRegister();
            vm.Env[id] = Value.Make(Core.Binding, new Register(0, r));
            return (id, r, b);
        }).ToArray();

        Args = args;
        Vararg = vararg;
    }

    public void BindArgs(VM vm, int arity, Stack stack)
    {
        for (var i = Args.Length - 1; i >= 0; i--)
        {
            if (Args[i].Item2 == -1) { continue; }

            if (Vararg && i == Args.Length - 1)
            {
                var n = arity - Args.Length + 1;
                var vs = new Value[n];
                for (var j = n - 1; j >= 0; j--) { vs[j] = stack.Pop(); }
                vm.SetRegister(0, Args[i].Item2, Value.Make(Core.Array, vs));
            }
            else
            {
                vm.SetRegister(0, Args[i].Item2, stack.Pop());
            }
        }

        foreach (var (r, v) in ClosureValues) { vm.SetRegister(0, r, v); }
    }

    public override string ToString()
    {
        var result = new StringBuilder();
        result.Append($"({Name} [");

        for (var i = 0; i < Args.Length; i++)
        {
            if (i > 0) { result.Append(' '); }
            result.Append(Args[i].Item1);
        }

        if (Vararg) { result.Append('*'); }
        result.Append("])");
        return result.ToString();
    }
}

