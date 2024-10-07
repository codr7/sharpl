namespace Sharpl.Libs;

using Sharpl.Types.Term;
using System.Text;

public class Term : Lib
{
    public static readonly KeyType Key = new KeyType("Key", [Core.Any]);

    public Term(VM vm) : base("term", null, [])
    {
        BindType(Key);

        Bind("DOWN", Value.Make(Term.Key, new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false)));
        Bind("ESC", Value.Make(Term.Key, new ConsoleKeyInfo('\u001B', ConsoleKey.Escape, false, false, false)));
        Bind("UP", Value.Make(Term.Key, new ConsoleKeyInfo('\0', ConsoleKey.UpArrow, false, false, false)));

        BindMethod("ask", ["prompt?"], (vm, stack, target, arity, loc) =>
        {
            if (vm.Term.Ask((arity == 1) ? stack.Pop().Say(vm) : null) is string s) { stack.Push(Core.String, s); }
        });

        BindMethod("clear-line", [], (vm, stack, target, arity, loc) =>
        {
            vm.Term.ClearLine();
        });

        BindMethod("clear-screen", [], (vm, stack, target, arity, loc) =>
        {
            vm.Term.ClearScreen();
        });

        BindMethod("flush", [], (vm, stack, target, arity, loc) =>
        {
            vm.Term.Flush();
        });

        BindMethod("height", [], (vm, stack, target, arity, loc) =>
        {
            stack.Push(Value.Make(Core.Int, vm.Term.Height));
        });

        BindMethod("move-to", ["x", "y"], (vm, stack, target, arity, loc) =>
        {
            var y = stack.Pop();
            var x = stack.Pop().CastUnbox(Core.Int, loc);
            vm.Term.MoveTo(x, (y == Value._) ? null : y.CastUnbox(Core.Int, loc));
        });

        BindMethod("poll-key", [], (vm, stack, target, arity, loc) =>
            stack.Push(Core.Bit, Console.KeyAvailable));

        BindMethod("read-key", [], (vm, stack, target, arity, loc) =>
        {
            vm.Term.Flush();
            Value? echo = (arity == 0) ? null : stack.Pop();

            var k = Console.ReadKey(true);

            if (echo is Value e)
            {
                if ((e.Type != Core.Bit) || e.CastUnbox(Core.Bit))
                {
                    Console.Write((e.Type == Core.Bit) ? k.KeyChar : e.Say(vm));
                }
            }

            stack.Push(Key, k);
        });

        BindMethod("read-line", ["echo"], (vm, stack, target, arity, loc) =>
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
                    if ((v.Type != Core.Bit) || v.CastUnbox(Core.Bit))
                    {
                        Console.Write((v.Type == Core.Bit) ? k.KeyChar : v.Say(vm));
                    }
                }

                res.Append(k.KeyChar);
            }

            stack.Push(Core.String, res.ToString());
        });

        BindMethod("reset", [], (vm, stack, target, arity, loc) =>
        {
            vm.Term.Reset();
        });

        BindMethod("restore", [], (vm, stack, target, arity, loc) =>
        {
            vm.Term.Restore();
        });

        BindMethod("save", [], (vm, stack, target, arity, loc) =>
        {
            vm.Term.Save();
        });

        BindMethod("set-region", [], (vm, stack, target, arity, loc) =>
        {
            if (arity == 0)
            {
                vm.Term.SetRegion();
                return;
            }

            stack.Reverse(arity);
            var min = (1, 1);
            var max = (vm.Term.Width, vm.Term.Height);

            if (arity > 0)
            {
                var p = stack.Pop().CastUnbox(Core.Pair, loc);
                min = (p.Item1.CastUnbox(Core.Int, loc), p.Item2.CastUnbox(Core.Int, loc));
                arity--;
            }

            if (arity > 0)
            {
                var p = stack.Pop().CastUnbox(Core.Pair, loc);
                max = (p.Item1.CastUnbox(Core.Int, loc), p.Item2.CastUnbox(Core.Int, loc));
                arity--;
            }

            vm.Term.SetRegion(min, max);
        });

        BindMethod("scroll-down", [], (vm, stack, target, arity, loc) =>
        {
            var lines = 1;

            if (arity > 0)
            {
                lines = stack.Pop().CastUnbox(Core.Int, loc);
                arity--;
            }

            vm.Term.ScrollDown(lines);
        });

        BindMethod("scroll-up", [], (vm, stack, target, arity, loc) =>
        {
            var lines = 1;

            if (arity > 0)
            {
                lines = stack.Pop().CastUnbox(Core.Int, loc);
                arity--;
            }

            vm.Term.ScrollUp(lines);
        });

        BindMethod("pick-back", ["color"], (vm, stack, target, arity, loc) =>
       {
           var c = stack.Pop().CastUnbox(Core.Color, loc);
           vm.Term.SetBg(c);
       });

        BindMethod("pick-front", ["color"], (vm, stack, target, arity, loc) =>
        {
            var c = stack.Pop().CastUnbox(Core.Color, loc);
            vm.Term.SetFg(c);
        });

        BindMethod("width", [], (vm, stack, target, arity, loc) =>
        {
            stack.Push(Value.Make(Core.Int, vm.Term.Width));
        });

        BindMethod("write", [], (vm, stack, target, arity, loc) =>
        {
            stack.Reverse(arity);
            var res = new StringBuilder();

            while (arity > 0)
            {
                stack.Pop().Say(vm, res);
                arity--;
            }

            vm.Term.Write(res.ToString());
        });
    }
}