namespace Sharpl.Libs;

using Sharpl.Types.Term;

using System.Text;

public class Term : Lib
{
    public static readonly KeyType Key = new KeyType("Key");

    public Term(VM vm) : base("term", null)
    {
        BindType(Key);

        var term = new Sharpl.Term();

        BindMethod("set-bg", ["color"], (loc, target, vm, stack, arity, recursive) =>
        {
            var c = stack.Pop().Cast(loc, Core.Color);
            term.SetBg(c);            
        });

        BindMethod("set-fg", ["color"], (loc, target, vm, stack, arity, recursive) =>
        {
            var c = stack.Pop().Cast(loc, Core.Color);
            term.SetFg(c);                        
        });

        BindMethod("read-key", ["echo"], (loc, target, vm, stack, arity, recursive) =>
        {
            Value? echo = (arity == 0) ? null : stack.Pop();
            
            var k = Console.ReadKey(true);

            if (echo is Value e)
            {
                if ((e.Type != Core.Bit) || e.Cast(Core.Bit))
                {
                    Console.Write((e.Type == Core.Bit) ? k.KeyChar : e.Say());
                }
            }

            stack.Push(Key, k);
        });

        BindMethod("read-line", ["echo"], (loc, target, vm, stack, arity, recursive) =>
        {
            Value? echo = (arity == 0) ? null : stack.Pop();
 
           var res = new StringBuilder();

            while (true)
            {
                var k = Console.ReadKey(true);

                if (k.Key == ConsoleKey.Enter)
                {
                    break;
                }

                if (echo is Value v)
                {
                    if ((v.Type != Core.Bit) || v.Cast(Core.Bit))
                    {
                        Console.Write((v.Type == Core.Bit) ? k.KeyChar : v.Say());
                    }
                }

                res.Append(k.KeyChar);
            }

            stack.Push(Core.String, res.ToString());
        });
    }
}