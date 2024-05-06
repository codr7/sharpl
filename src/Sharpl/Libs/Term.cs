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

        BindMethod("read-key", ["echo"], (loc, target, vm, stack, arity, recursive) =>
        {
            Value? echo = (arity == 0) ? null : stack.Pop();
            var k = Console.ReadKey(true);

            if (echo is Value v)
            {
                if ((v.Type != Core.Bit) || v.Cast(Core.Bit))
                {
                    Console.Write((v.Type == Core.Bit) ? k.KeyChar : v.Say());
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

        BindMethod("say", [], (loc, target, vm, stack, arity, recursive) =>
        {
            stack.Reverse(arity);
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