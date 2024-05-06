namespace Sharpl.Libs;

using Sharpl.Types.Term;

using System.Text;

public class Term : Lib
{
    public static readonly KeyType Key = new KeyType("Int");
 
    public Term() : base("term", null)
    {
        BindType(Key);

        BindMethod("b-rgb", [], (loc, target, vm, stack, arity, recursive) =>
        {
            var res = new StringBuilder();
            res.Append((char)27);
            var b = stack.Pop().Cast(loc, Core.Int);
            var g = stack.Pop().Cast(loc, Core.Int);
            var r = stack.Pop().Cast(loc, Core.Int);
            res.Append($"[48;2;{r};{g};{b}m");            
            stack.Push(Core.String, res.ToString());
        });

        BindMethod("f-rgb", [], (loc, target, vm, stack, arity, recursive) =>
        {
            var res = new StringBuilder();
            res.Append((char)27);
            var b = stack.Pop().Cast(loc, Core.Int);
            var g = stack.Pop().Cast(loc, Core.Int);
            var r = stack.Pop().Cast(loc, Core.Int);
            res.Append($"[38;2;{r};{g};{b}m");            
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