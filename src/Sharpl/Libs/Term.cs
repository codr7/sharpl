namespace Sharpl.Libs;

using Sharpl.Types.Term;

using System.Text;

public class Term : Lib
{
    public static readonly KeyType Key = new KeyType("Int");
 
    public Term() : base("term", null)
    {
        BindType(Key);

        BindMethod("set-bg", ["color"], (loc, target, vm, stack, arity, recursive) =>
        {
            var res = new StringBuilder();
            res.Append((char)27);
            var c = stack.Pop().Cast(loc, Core.Color);
            res.Append($"[48;2;{c.R};{c.G};{c.B}m");            
            stack.Push(Core.String, res.ToString());
        });

        BindMethod("set-fg", ["color"], (loc, target, vm, stack, arity, recursive) =>
        {
            var res = new StringBuilder();
            res.Append((char)27);
            var c = stack.Pop().Cast(loc, Core.Color);
            res.Append($"[38;2;{c.R};{c.G};{c.B}m");            
            stack.Push(Core.String, res.ToString());
        });

        BindMethod("read-key", [], (loc, target, vm, stack, arity, recursive) =>
        {
            stack.Push(Key, Console.ReadKey(true));
        });

        BindMethod("read-line", [], (loc, target, vm, stack, arity, recursive) =>
        {
            var res = Console.ReadLine();

            if (res is null) {
                stack.Push(Core.Nil, false);
            } else {
                stack.Push(Core.String, res);
            }
        });

        BindMethod("say", [], (loc, target, vm, stack, arity, recursive) =>
        {
            var res = new StringBuilder();

            while (arity > 0)
            {
                stack.Pop().Say(res);
                arity--;
            }

            Console.WriteLine(res.ToString());
        });
    }
}