using System.Data;
using System.Text;
using Sharpl.Libs;

namespace Sharpl;

public class UserMethod
{
    public readonly (string, int)[] Args;
    public readonly int MinArgCount;
    public readonly (string, int, Register)[] Closure;
    public readonly Dictionary<int, Value> ClosureValues = new Dictionary<int, Value>();
    public readonly Loc Loc;
    public readonly string Name;
    public int? StartPC;
    public readonly bool Vararg;

    public UserMethod(VM vm, string name, string[] ids, (string, int)[] args, bool vararg, Loc loc)
    {
        Loc = loc;
        Name = name;

        Closure = ids.AsEnumerable().Select<string, (string, int, Register)>(id =>
        {
#pragma warning disable CS8629
            var b = ((Value)vm.Env[id]).CastUnbox(Core.Binding);
#pragma warning restore CS8629
            var r = vm.AllocRegister();
            vm.Env[id] = Value.Make(Core.Binding, new Register(0, r));
            return (id, r, b);
        }).ToArray();

        Args = args;
        MinArgCount = args[0..(vararg ? ^1 : ^0)].Count((a) => !a.Item1.EndsWith('?'));
        Vararg = vararg;
    }

    public void BindArgs(VM vm, Value?[] argMask, int arity, Stack stack)
    {
        for (var i = Args.Length - 1; i >= 0; i--)
        {
            if (i >= argMask.Length) { break; }
            var ar = Args[i].Item2;
            if (ar == -1) { continue; }

            if (Vararg && i == Args.Length - 1)
            {
                var n = arity - Args.Length + 1;
                var vs = new Value[n];
                for (var j = n - 1; j >= 0; j--) { vs[j] = (argMask[n+j-2] is Value v) ? v : stack.Pop(); }
                vm.SetRegister(0, ar, Value.Make(Core.Array, vs));
            }
            else
            {
                if (argMask[i] is Value v)
                {
                    if (v.Type == Core.Binding)
                    {
                        var r = v.CastUnbox(Core.Binding);
                        if (r.FrameOffset != 0 || r.Index != ar) { vm.SetRegister(0, ar, vm.Get(r)); }
                    }
                    else { vm.SetRegister(0, ar, v.Copy()); }
                }
                else { vm.SetRegister(0, ar, stack.Pop()); }
            }
        }

        foreach (var (r, v) in ClosureValues) { vm.SetRegister(0, r, v); }
    }

    public override string ToString()
    {
        var result = new StringBuilder();
        result.Append($"(^{Name} [");

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

