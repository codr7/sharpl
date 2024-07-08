namespace Sharpl.Libs;

using Sharpl.Types.Term;
using System.Drawing;
using System.Text;

public class Term : Lib
{
    public static readonly KeyType Key = new KeyType("Key");

    public Term(VM vm) : base("term", null, [])
    {
        BindType(Key);

        BindMethod("clear-line", [], (loc, target, vm, stack, arity) =>
        {
            vm.Term.ClearLine();
        });

        BindMethod("clear-screen", [], (loc, target, vm, stack, arity) =>
        {
            vm.Term.ClearScreen();
        });

        BindMethod("flush", [], (loc, target, vm, stack, arity) =>
        {
            vm.Term.Flush();
        });

        BindMethod("height", [], (loc, target, vm, stack, arity) =>
        {
            stack.Push(Value.Make(Core.Int, vm.Term.Height));
        });

        BindMethod("move-to", ["x", "y"], (loc, target, vm, stack, arity) =>
        {
            var y = stack.Pop();
            var x = stack.Pop().Cast(loc, Core.Int);
            vm.Term.MoveTo(x, (y == Value.Nil) ? null : y.Cast(loc, Core.Int));
        });

        BindMethod("read-key", [], (loc, target, vm, stack, arity) =>
        {
            vm.Term.Flush();
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

        BindMethod("read-line", ["echo"], (loc, target, vm, stack, arity) =>
        {
            vm.Term.Flush();
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

        BindMethod("reset", [], (loc, target, vm, stack, arity) =>
        {
            vm.Term.Reset();
        });

        BindMethod("restore", [], (loc, target, vm, stack, arity) =>
        {
            vm.Term.Restore();
        });

        BindMethod("save", [], (loc, target, vm, stack, arity) =>
        {
            vm.Term.Save();
        });

        BindMethod("set-bg", ["color"], (loc, target, vm, stack, arity) =>
       {
           var c = stack.Pop().Cast(loc, Core.Color);
           vm.Term.SetBg(c);
       });

        BindMethod("set-fg", ["color"], (loc, target, vm, stack, arity) =>
        {
            var c = stack.Pop().Cast(loc, Core.Color);
            vm.Term.SetFg(c);
        });

        BindMethod("width", [], (loc, target, vm, stack, arity) =>
        {
            stack.Push(Value.Make(Core.Int, vm.Term.Width));
        });

        BindMethod("write", [], (loc, target, vm, stack, arity) =>
        {
            stack.Reverse(arity);
            var res = new StringBuilder();

            while (arity > 0)
            {
                stack.Pop().Say(res);
                arity--;
            }

            vm.Term.Write(res.ToString());
        });
    }
}