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
        Bind("LEFT", Value.Make(Term.Key, new ConsoleKeyInfo('\0', ConsoleKey.LeftArrow, false, false, false)));
        Bind("ENTER", Value.Make(Term.Key, new ConsoleKeyInfo('\0', ConsoleKey.Enter, false, false, false)));
        Bind("ESC", Value.Make(Term.Key, new ConsoleKeyInfo('\u001B', ConsoleKey.Escape, false, false, false)));
        Bind("RIGHT", Value.Make(Term.Key, new ConsoleKeyInfo('\0', ConsoleKey.RightArrow, false, false, false)));
        Bind("SPACE", Value.Make(Term.Key, new ConsoleKeyInfo('\0', ConsoleKey.Spacebar, false, false, false)));
        Bind("UP", Value.Make(Term.Key, new ConsoleKeyInfo('\0', ConsoleKey.UpArrow, false, false, false)));

        BindMethod("ask", ["prompt?", "echo?"], (vm, target, arity, result, loc) =>
        {
            Value? echo = (arity > 1) ? vm.GetRegister(0, 1) : null;
            string? prompt = (arity > 0) ? vm.GetRegister(0, 0).Say(vm) : null;
            if (vm.Term.Ask(vm, prompt, echo) is string s) vm.Set(result, Value.Make(Core.String, s));   
        });

        BindMethod("clear-line", [], (vm, target, arity, result, loc) => vm.Term.ClearLine());

        BindMethod("clear-screen", [], (vm, target, arity, result, loc) => vm.Term.ClearScreen());

        BindMethod("flush", [], (vm, target, arity, result, loc) => vm.Term.Flush());

        BindMethod("height", [], (vm, target, arity, result, loc) =>
            vm.Set(result, Value.Make(Core.Int, vm.Term.Height)));

        BindMethod("move-to", ["x", "y"], (vm, target, arity, result, loc) =>
        {
            var x = vm.GetRegister(0, 0).CastUnbox(Core.Int, loc);
            var y = vm.GetRegister(0, 1);
            vm.Term.MoveTo(x, (y == Value._) ? null : y.CastUnbox(Core.Int, loc));
        });

        BindMethod("poll-key", [], (vm, target, arity, result, loc) =>
            vm.Set(result, Value.Make(Core.Bit, Console.KeyAvailable)));

        BindMethod("read-key", [], (vm, target, arity, result, loc) =>
        {
            vm.Term.Flush();
            Value? echo = (arity == 0) ? null : vm.GetRegister(0, 0);
            var k = Console.ReadKey(true);

            if (echo is Value e)
            {
                if ((e.Type != Core.Bit) || e.CastUnbox(Core.Bit))
                    Console.Write((e.Type == Core.Bit) ? k.KeyChar : e.Say(vm));
            }

            vm.Set(result, Value.Make(Key, k));
        });

        BindMethod("reset", [], (vm, target, arity, result, loc) => vm.Term.Reset());
        BindMethod("restore", [], (vm, stack, target, arity, loc) => vm.Term.Restore());
        BindMethod("save", [], (vm, stack, target, arity, loc) => vm.Term.Save());

        BindMethod("set-region", [], (vm, target, arity, result, loc) =>
        {
            if (arity == 0)
            {
                vm.Term.SetRegion();
                return;
            }

            var min = (1, 1);
            var max = (vm.Term.Width, vm.Term.Height);

            if (arity > 0)
            {
                var p = vm.GetRegister(0, 0).CastUnbox(Core.Pair, loc);
                min = (p.Item1.CastUnbox(Core.Int, loc), p.Item2.CastUnbox(Core.Int, loc));
            }

            if (arity > 1)
            {
                var p = vm.GetRegister(0, 1).CastUnbox(Core.Pair, loc);
                max = (p.Item1.CastUnbox(Core.Int, loc), p.Item2.CastUnbox(Core.Int, loc));
            }

            vm.Term.SetRegion(min, max);
        });

        BindMethod("scroll-down", ["lines?"], (vm, target, arity, result, loc) =>
        {
            var lines = 1;
            if (arity > 0) lines = vm.GetRegister(0, 0).CastUnbox(Core.Int, loc);
            vm.Term.ScrollDown(lines);
        });

        BindMethod("scroll-up", ["lines?"], (vm, target, arity, result, loc) =>
        {
            var lines = 1;
            if (arity > 0) lines = vm.GetRegister(0, 0).CastUnbox(Core.Int, loc);
            vm.Term.ScrollUp(lines);
        });

        BindMethod("pick-back", ["color"], (vm, target, arity, result, loc) =>
       {
           var c = vm.GetRegister(0, 0).CastUnbox(Core.Color, loc);
           vm.Term.SetBg(c);
       });

        BindMethod("pick-front", ["color"], (vm, target, arity, result, loc) =>
        {
            var c = vm.GetRegister(0, 0).CastUnbox(Core.Color, loc);
            vm.Term.SetFg(c);
        });

        BindMethod("width", [], (vm, target, arity, result, loc) =>
            vm.Set(result, (Value.Make(Core.Int, vm.Term.Width))));

        BindMethod("write", [], (vm, target, arity, result, loc) =>
        {
            var res = new StringBuilder();
            for (var i = 0; i < arity; i++) vm.GetRegister(0, i).Say(vm, res);
            vm.Term.Write(res.ToString());
        });
    }
}