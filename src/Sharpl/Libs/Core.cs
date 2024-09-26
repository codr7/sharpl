using Sharpl.Types.Core;
using System.Text;
using Forms = Sharpl.Forms;

namespace Sharpl.Libs;

public class Core : Lib
{
    public static readonly ArrayType Array = new ArrayType("Array");
    public static readonly BindingType Binding = new BindingType("Binding");
    public static readonly BitType Bit = new BitType("Bit");
    public static readonly CharType Char = new CharType("Char");
    public static readonly ColorType Color = new ColorType("Color");
    public static readonly DurationType Duration = new DurationType("Duration");
    public static readonly FixType Fix = new FixType("Fix");
    public static readonly FormType Form = new FormType("Form");
    public static readonly IntType Int = new IntType("Int");
    public static readonly IterType Iter = new IterType("Iter");
    public static readonly LibType Lib = new LibType("Lib");
    public static readonly ListType List = new ListType("List");
    public static readonly MacroType Macro = new MacroType("Macro");
    public static readonly MapType Map = new MapType("Map");
    public static readonly MetaType Meta = new MetaType("Meta");
    public static readonly MethodType Method = new MethodType("Method");
    public static readonly NilType Nil = new NilType("Nil");
    public static readonly PairType Pair = new PairType("Pair");
    public static readonly PipeType Pipe = new PipeType("Pipe");
    public static readonly PortType Port = new PortType("Port");
    public static readonly StringType String = new StringType("String");
    public static readonly SymType Sym = new SymType("Sym");
    public static readonly TimestampType Timestamp = new TimestampType("Timestamp");
    public static readonly UserMethodType UserMethod = new UserMethodType("UserMethod");

    public static void DefineMethod(Loc loc, VM vm, Form.Queue args, Type<UserMethod> type, Op stopOp)
    {
        var name = "";
        var f = args.TryPop();

        if (f is Forms.Id id)
        {
            name = id.Name;
            f = args.TryPop();
        }

        (string, int)[] fas;
        var parentEnv = vm.Env;
        var registerCount = vm.NextRegisterIndex;

        var ids = args.CollectIds().Where(id =>
           vm.Env[id] is Value v &&
           v.Type == Binding &&
           v.CastUnbox(Binding).FrameOffset != -1).
           ToHashSet<string>();

        bool vararg = false;

        vm.DoEnv(new Env(vm.Env, ids), () =>
        {
            if (f is Forms.Array af)
            {
                fas = af.Items.Select(f =>
                {
                    if (vararg) { throw new EmitError("Vararg must be final param.", f.Loc); }

                    if (f is Forms.Splat s)
                    {
                        f = s.Target;
                        vararg = true;
                    }

                    if (f is Forms.Id id)
                    {
                        var r = vm.AllocRegister();
                        vm.Env.Bind(id.Name, Value.Make(Binding, new Register(0, r)));
                        return (id.Name, r);
                    }

                    if (f is Forms.Nil)
                    {
                        return ("_", -1);
                    }

                    throw new EmitError($"Invalid method arg: {f.Dump(vm)}", f.Loc);
                }).ToArray();
            }
            else { throw new EmitError($"Invalid method args: {f!.Dump(vm)}", loc); }

            var m = new UserMethod(vm, name, ids.ToArray(), fas, vararg, loc);
            var skip = new Label();
            if (ids.Count > 0) { vm.Emit(Ops.PrepareClosure.Make(m, skip)); }
            else { vm.Emit(Ops.Goto.Make(skip)); }
            m.StartPC = vm.EmitPC;
            var v = Value.Make(type, m);
            if (name != "") { parentEnv.Bind(name, v); }
            args.Emit(vm);
            m.EndPC = vm.EmitPC;
            vm.Emit(stopOp);
            skip.PC = vm.EmitPC;
            if (name == "") { v.Emit(vm, args, loc); }
        });
    }

    public Core() : base("core", null, [])
    {
        BindType(Array);
        BindType(Binding);
        BindType(Bit);
        BindType(Char);
        BindType(Color);
        BindType(Duration);
        BindType(Fix);
        BindType(Int);
        BindType(Lib);
        BindType(List);
        BindType(Macro);
        BindType(Map);
        BindType(Meta);
        BindType(Method);
        BindType(Pair);
        BindType(Pipe);
        BindType(Port);
        BindType(String);
        BindType(Sym);
        BindType(Timestamp);
        BindType(UserMethod);

        Bind("F", Value.F);
        Bind("T", Value.T);

        BindMacro("^", [], (vm, target, args, loc) =>
            DefineMethod(loc, vm, args, UserMethod, Ops.ExitMethod.Make()));

        BindMethod("=", ["x"], (vm, stack, target, arity, loc) =>
        {
            var v = stack.Pop();
            arity--;
            var res = true;

            while (arity > 0)
            {
                if (!stack.Pop().Equals(v))
                {
                    res = false;
                    break;
                }

                arity--;
            }

            stack.Push(Value.Make(Bit, res));
        });

        BindMethod("<", ["x", "y"], (vm, stack, target, arity, loc) =>
        {
            var lv = stack.Pop();
            arity--;
            var res = true;
            if (lv.Type is not ComparableTrait t) { throw new EvalError($"Not comparable: {lv}", loc); }

            while (arity > 0)
            {
                var rv = stack.Pop();
                if (rv.Type != t) { throw new EvalError($"Type mismatch: {lv} {rv}", loc); }

                if (t.Compare(lv, rv) != Order.GT)
                {
                    res = false;
                    break;
                }

                arity--;
            }

            stack.Push(Value.Make(Bit, res));
        });

        BindMethod(">", ["x", "y"], (vm, stack, target, arity, loc) =>
        {
            var lv = stack.Pop();
            arity--;
            var res = true;
            var t = lv.Type as ComparableTrait;
            if (t is null) { throw new EvalError($"Not comparable: {lv}", loc); }

            while (arity > 0)
            {
                var rv = stack.Pop();
                if (rv.Type != t) { throw new EvalError($"Type mismatch: {lv} {rv}", loc); }

                if (t.Compare(lv, rv) != Order.LT)
                {
                    res = false;
                    break;
                }

                arity--;
            }

            stack.Push(Value.Make(Bit, res));
        });


        BindMethod("+", ["x"], (vm, stack, target, arity, loc) =>
        {
            if (stack[^arity].Type is NumericTrait nt) { nt.Add(loc, vm, stack, arity); }
            else { throw new EvalError($"Expected numeric value", loc); }
        });

        BindMethod("-", ["x"], (vm, stack, target, arity, loc) =>
        {
            if (stack[^arity].Type is NumericTrait nt) { nt.Subtract(loc, vm, stack, arity); }
            else { throw new EvalError($"Expected numeric value", loc); }
        });

        BindMethod("*", ["x", "y"], (vm, stack, target, arity, loc) =>
         {
             if (stack[^arity].Type is NumericTrait nt)
             { nt.Multiply(loc, vm, stack, arity); }
             else { throw new EvalError($"Expected numeric value", loc); }
         });

        BindMethod("/", ["x", "y"], (vm, stack, target, arity, loc) =>
        {
            if (stack[^arity].Type is NumericTrait nt) { nt.Divide(loc, vm, stack, arity); }
            else { throw new EvalError($"Expected numeric value", loc); }
        });

        BindMacro("and", ["value1"], (vm, target, args, loc) =>
             {
                 var done = new Label();
                 var first = true;

                 while (!args.Empty)
                 {
                     if (!first) { vm.Emit(Ops.Drop.Make(1)); }
                     vm.Emit(args.Pop());

                     if (!args.Empty)
                     {
                         vm.Emit(Ops.And.Make(done));
                         first = false;
                     }
                 }

                 done.PC = vm.EmitPC;
             });

        BindMacro("bench", ["n"], (vm, target, args, loc) =>
         {
             if (args.TryPop() is Form f && vm.Eval(f) is Value n)
             {
                 vm.Emit(Ops.Benchmark.Make(n.CastUnbox(Int, loc)));
                 args.Emit(vm);
                 vm.Emit(Ops.Stop.Make());
             }
             else { throw new EmitError("Missing repetitions", loc); }
         });

        BindMacro("check", ["x"], (vm, target, args, loc) =>
         {
             var ef = args.Pop();
             var emptyArgs = true;

             if (!args.Empty)
             {
                 vm.Emit(Ops.BeginFrame.Make(vm.NextRegisterIndex));

                 vm.DoEnv(new Env(vm.Env, args.CollectIds()), () =>
                 {
                     while (true)
                     {
                         if (args.TryPop() is Form bf) { vm.Emit(bf); }
                         else { break; }
                     }

                 });

                 vm.Emit(Ops.EndFrame.Make());
                 emptyArgs = false;
             }

             vm.Emit(ef);
             if (emptyArgs) { Value.T.Emit(vm, new Form.Queue(), loc); }
             vm.Emit(Ops.Check.Make(loc));
         });

        BindMethod("close", ["it"], (vm, stack, target, arity, loc) =>
        {
            var it = stack.Pop();
            if (it.Type is CloseTrait ct) { ct.Close(it); }
            else { throw new EvalError($"Not supported: {it}", loc); }
        });

        BindMacro("dec", ["delta?"], (vm, target, args, loc) =>
        {
            if (args.TryPop() is Forms.Id id && vm.Env[id.Name] is Value v && v.Type == Binding)
            {
                var r = v.CastUnbox(Binding);
                var d = args.Empty ? null : args.Pop();
                if (d is not null && d is not Forms.Literal) { throw new EmitError($"Literal expected: {d}", d.Loc); }
                vm.Emit(Ops.Decrement.Make(r, (d is null) ? 1 : (d as Forms.Literal)!.Value.CastUnbox(Int, loc)));
                vm.Emit(Ops.GetRegister.Make(r));
            }
            else { throw new EmitError("Invalid target", loc); }
        });

        BindMacro("dmit", [], (vm, target, args, loc) =>
        {
            var skip = new Label();
            vm.Emit(Ops.Goto.Make(skip));
            var startPC = vm.EmitPC;
            foreach (var a in args) { vm.Emit(a.Unquote(vm, loc)); }
            args.Clear();
            skip.PC = vm.EmitPC;
            vm.Emit(Ops.Stop.Make());
            vm.Dmit(startPC);
        });

        BindMacro("do", [], (vm, target, args, loc) =>
        {
            vm.Emit(Ops.BeginFrame.Make(vm.NextRegisterIndex));
            vm.DoEnv(new Env(vm.Env, args.CollectIds()), () => args.Emit(vm));
            vm.Emit(Ops.EndFrame.Make());
        });

        BindMacro("else", ["condition", "true?", "false?"], (vm, target, args, loc) =>
             {
                 if (args.TryPop() is Form cf) { vm.Emit(cf); }
                 else { throw new EmitError("Missing condition", loc); }
                 var skipElse = new Label();
                 vm.Emit(Ops.Branch.Make(skipElse, loc));
                 if (args.TryPop() is Form tf) { vm.Emit(tf); }
                 var skipEnd = new Label();
                 vm.Emit(Ops.Goto.Make(skipEnd));
                 skipElse.PC = vm.EmitPC;
                 args.Emit(vm);
                 skipEnd.PC = vm.EmitPC;
             });

        BindMacro("emit", ["code?"], (vm, target, args, loc) =>
        {
            var skip = new Label();
            vm.Emit(Ops.Goto.Make(skip));
            var startPC = vm.EmitPC;
            foreach (var a in args) { vm.Emit(a.Unquote(vm, loc)); }
            args.Clear();
            vm.Emit(Ops.Stop.Make());
            skip.PC = vm.EmitPC;

            var stack = new Stack();
            vm.Eval(startPC, stack);
            stack.Reverse();

            foreach (var it in stack) { args.PushFirst(new Forms.Literal(it, loc)); }
        });

        BindMethod("eval", ["code?"], (vm, stack, target, arity, loc) =>
        {
            var f = stack.Pop().Unquote(vm, loc);
            vm.Eval(f, stack);
        });

        BindMethod("exit", ["code?"], (vm, stack, target, arity, loc) =>
            Environment.Exit((arity == 0) ? 0 : stack.Pop().CastUnbox(Int)));

        BindMethod("fail", [], (vm, stack, target, arity, loc) =>
        {
            stack.Reverse(arity);
            var res = new StringBuilder();

            while (arity > 0)
            {
                stack.Pop().Say(vm, res);
                arity--;
            }

            throw new EvalError(res.ToString(), loc);
        });

        BindMethod("gensym", ["name"], (vm, stack, target, arity, loc) =>
            {
                stack.Reverse(arity);
                var res = new StringBuilder();

                while (arity > 0)
                {
                    stack.Pop().Say(vm, res);
                    arity--;
                }

                stack.Push(Value.Make(Sym, vm.Gensym(res.ToString())));
            });

        BindMacro("if", ["condition"], (vm, target, args, loc) =>
            {
                if (args.TryPop() is Form f) { vm.Emit(f); }
                else { throw new EmitError("Missing condition", loc); }

                var skip = new Label();
                vm.Emit(Ops.Branch.Make(skip, loc));
                args.Emit(vm);
                skip.PC = vm.EmitPC;
            });

        BindMacro("inc", ["delta?"], (vm, target, args, loc) =>
        {
            if (args.TryPop() is Forms.Id id && vm.Env[id.Name] is Value v && v.Type == Binding)
            {
                var r = v.CastUnbox(Binding);
                var d = args.Empty ? null : args.Pop();
                if (d is not null && d is not Forms.Literal) { throw new EmitError($"Literal expected: {d}", d!.Loc); }
                vm.Emit(Ops.Increment.Make(r, (d is null) ? 1 : (d as Forms.Literal)!.Value.CastUnbox(Int, loc)));
                vm.Emit(Ops.GetRegister.Make(r));
            }
            else { throw new EmitError("Invalid target", loc); }
        });

        BindMethod("is", ["x"], (vm, stack, target, arity, loc) =>
            {
                var v = stack.Pop();
                arity--;
                var res = true;

                while (arity > 0)
                {
                    var sv = stack.Pop();

                    if (sv.Type != v.Type || !sv.Data.Equals(v.Data))
                    {
                        res = false;
                        break;
                    }

                    arity--;
                }

                stack.Push(Value.Make(Bit, res));
            });

        BindMethod("length", ["it"], (vm, stack, target, arity, loc) =>
            {
                var v = stack.Pop();
                if (v.Type is LengthTrait st) { stack.Push(Int, st.Length(v)); }
                else { throw new EvalError($"Not supported: {v}", loc); }
            });

        BindMacro("let", ["bindings"], (vm, target, args, loc) =>
            {
                var ids = args.CollectIds();

                if (args.TryPop() is Forms.Array bsf)
                {
                    var bs = bsf.Items;

                    for (var i = 0; i < bs.Length; i += 2)
                    {
                        if (bs[i] is Forms.Id f) { ids.Remove(f.Name); }
                    }

                    vm.Emit(Ops.BeginFrame.Make(vm.NextRegisterIndex));

                    vm.DoEnv(new Env(vm.Env, ids), () =>
                     {
                         var brs = new List<(int, int, int)>();

                         for (var i = 0; i < bs.Length; i++)
                         {
                             var f = bs[i];
                             i++;
                             vm.Emit(bs[i]);
                             bind(vm, f, brs);
                         }

                         while (true)
                         {
                             if (args.TryPop() is Form f) { vm.Emit(f); }
                             else { break; }
                         }

                         foreach (var (fromIndex, toFrameOffset, toIndex) in brs)
                         {
                             vm.Emit(Ops.CopyRegister.Make(new Register(0, fromIndex), new Register(toFrameOffset, toIndex)));
                         }

                         vm.Emit(Ops.EndFrame.Make());
                     });
                }
                else { throw new EmitError("Missing bindings", loc); }
            });

        BindMacro("lib", [], (vm, target, args, loc) =>
            {
                if (args.Count == 0) { vm.Emit(Ops.Push.Make(Value.Make(Lib, vm.Lib))); }

                else if (args.TryPop() is Forms.Id nf)
                {
                    Lib? lib = null;

                    if (vm.Env.Find(nf.Name) is Value v) { lib = v.Cast(Lib, loc); }
                    else
                    {
                        lib = new Lib(nf.Name, vm.Env, args.CollectIds());
                        vm.Env.BindLib(lib);
                    }

                    if (args.Empty) { vm.Env = lib; }
                    else { vm.DoEnv(lib, () => args.Emit(vm)); }
                }
                else { throw new EmitError("Invalid library name", loc); }
            });

        BindMacro("load", ["path"], (vm, target, args, loc) =>
            {
                while (true)
                {
                    if (args.TryPop() is Form pf)
                    {
                        if (vm.Eval(pf) is Value p) { vm.Load(p.Cast(String, pf.Loc)); }
                        else { throw new EvalError("Missing path", pf.Loc); }
                    }
                    else { break; }
                }
            });

        BindMacro("loop", ["body?"], (vm, target, args, loc) =>
        {
            vm.Emit(Ops.BeginFrame.Make(vm.NextRegisterIndex));

            vm.DoEnv(new Env(vm.Env, args.CollectIds()), () =>
             {
                 var end = new Label();
                 var start = new Label(vm.EmitPC);
                 args.Emit(vm);
                 vm.Emit(Ops.Goto.Make(start));
                 end.PC = vm.EmitPC;
                 vm.Emit(Ops.EndFrame.Make());
             });
        });

        BindMethod("max", ["x", "y?"], (vm, stack, target, arity, loc) =>
        {
            var v = stack.Pop();
            var t = v.Type as ComparableTrait;
            arity--;

            while (arity > 0)
            {
                var sv = stack.Pop();
                if (sv.Type != t) { throw new EvalError("Wrong type: {sv.Type}/{t}", loc); }
                if (t.Compare(sv, v) != Order.LT) { v = sv; }
                arity--;
            }

            stack.Push(v);
        });

        BindMethod("min", ["x", "y?"], (vm, stack, target, arity, loc) =>
        {
            var v = stack.Pop();
            var t = v.Type as ComparableTrait;
            arity--;

            while (arity > 0)
            {
                var sv = stack.Pop();
                if (sv.Type != t) { throw new EvalError("Wrong type: {sv.Type}/{t}", loc); }
                if (t.Compare(sv, v) != Order.GT) { v = sv; }
                arity--;
            }

            stack.Push(v);
        });

        BindMethod("not", ["it"], (vm, stack, target, arity, loc) => stack.Push(Bit, !(bool)stack.Pop()));

        BindMacro("or", ["value1"], (vm, target, args, loc) =>
             {
                 var done = new Label();
                 var first = true;

                 while (!args.Empty)
                 {
                     if (!first) { vm.Emit(Ops.Drop.Make(1)); }
                     vm.Emit(args.Pop());

                     if (!args.Empty)
                     {
                         vm.Emit(Ops.Or.Make(done));
                         first = false;
                     }
                 }

                 done.PC = vm.EmitPC;
             });

        BindMethod("parse-int", ["val"], (vm, stack, target, arity, loc) =>
        {
            int v = 0;
            var i = 0;

            foreach (char c in stack.Pop().Cast(String))
            {

                if (!char.IsDigit(c)) { break; }
                v = v * 10 + c - '0';
                i++;
            }

            stack.Push(Pair, (Value.Make(Int, v), Value.Make(Int, i)));
        });

        BindMethod("peek", ["src"], (vm, stack, target, arity, loc) =>
        {
            var src = stack.Pop();
            if (src.Type is StackTrait st) { stack.Push(st.Peek(loc, vm, src)); }
            else { throw new EvalError("Invalid peek target: {src}", loc); }
        });

        BindMethod("poll", ["source1", "source2?"], (vm, stack, target, arity, loc) =>
        {
            var ss = new (Task<bool>, Value)[arity];
            var cts = new CancellationTokenSource();

            for (var i = arity - 1; i >= 0; i--)
            {
                var s = stack.Pop();
                if (s.Type is PollTrait pt) { ss[i] = (pt.Poll(s, cts.Token), s); }
                else { throw new EvalError($"Not pollable: {s.Dump(vm)}", loc); }
            }

            stack.Push(Task.Run(async () => await TaskUtil.Any(ss, cts)).Result);
        });

        BindMacro("pop", ["src"], (vm, target, args, loc) =>
         {
             var src = args.Pop();

             if (src is Forms.Id id && vm.Env[id.Name] is Value v && v.Type == Binding)
             {
                 vm.Emit(Ops.PopItem.Make(v.CastUnbox(Binding), loc));
             }
             else { throw new EmitError($"Invalid push destination: {src}", loc); }
         });

        BindMacro("push", ["dst", "val"], (vm, target, args, loc) =>
         {
             var dst = args.Pop();

             if (dst is Forms.Id id && vm.Env[id.Name] is Value v && v.Type == Binding)
             {
                 foreach (var a in args)
                 {
                     vm.Emit(a);
                     vm.Emit(Ops.PushItem.Make(v.CastUnbox(Binding), loc));
                 }
             }
             else { throw new EmitError($"Invalid push destination: {dst}", loc); }
         });

        BindMethod("rand-int", ["n?"], (vm, stack, target, arity, loc) =>
        {
            Value? n = (arity == 1) ? stack.Pop() : null;
            var v = vm.Random.Next();
            stack.Push(Int, (n is null) ? v : v % ((Value)n).CastUnbox(Int, loc));
        });

        BindMethod("range", ["max", "min?", "stride?"], (vm, stack, target, arity, loc) =>
            {
                Value max = Value._, min = Value._, stride = Value._;
                AnyType t = Nil;

                switch (arity)
                {
                    case 1:
                        min = stack.Pop();
                        t = (min.Type == Nil || t != Nil) ? t : min.Type;
                        break;
                    case 2:
                        max = stack.Pop();
                        t = (max.Type == Nil) ? t : max.Type;
                        goto case 1;
                    case 3:
                        stride = stack.Pop();
                        t = (stride.Type == Nil || t != Nil) ? t : stride.Type;
                        goto case 2;
                }

                if (t is RangeTrait rt) { stack.Push(Iter, rt.CreateRange(loc, min, max, stride)); }
                else { throw new EvalError($"Invalid range type: {t}", loc); }
            });

        BindMethod("resize", ["array", "size", "value?"], (vm, stack, target, arity, loc) =>
        {
            var v = (arity == 3) ? stack.Pop() : Value._;
            var s = stack.Pop().CastUnbox(Int, loc);
            var a = stack.Pop().Cast(Array, loc);
            var ps = a.Length;
            System.Array.Resize(ref a, s);
            for (var i = ps; i < s; i++) { a[i] = v; }
            stack.Push(Array, a);
        });

        BindMacro("return", [], (vm, target, args, loc) =>
            {
                var vf = args.TryPop();
                UserMethod? m = null;

                if (vf is Forms.Call c)
                {
                    if (c.Target is Forms.Literal lt && lt.Value is var lv && lv.Type == UserMethod) { m = lv.Cast(UserMethod); }
                    else if (c.Target is Forms.Id it && vm.Env[it.Name] is Value iv && iv.Type == UserMethod) { m = iv.Cast(UserMethod); }

                    if (m is UserMethod)
                    {
                        if (!args.Empty) { throw new EmitError("Too many args in tail call", loc); }
                        var emptyArgs = new Form.Queue();
                        var splat = false;

                        foreach (var f in c.Args)
                        {
                            if (f.IsSplat)
                            {
                                splat = true;
                                break;
                            }
                        }

                        if (!splat && c.Args.Length < m.MinArgCount) { throw new EmitError($"Not enough arguments: {m}", loc); }
                        var argMask = new Value?[c.Args.Length];
                        var i = 0;

                        foreach (var f in c.Args)
                        {
                            /* Referring to the previous frame's bindings is tricky when we're not creating new frames.*/
                            if (f.GetValue(vm) is Value v && (v.Type != Binding || v.CastUnbox(Binding).FrameOffset != 0)) { argMask[i] = v; }
                            else { vm.Emit(f); }
                            i++;
                        }

                        vm.Emit(Ops.CallTail.Make(m, argMask, splat, loc));
                    }
                }

                if (m is null)
                {
                    if (vf is not null) { vm.Emit(vf); }
                    vm.Emit(Ops.ExitMethod.Make());
                }
            });

        BindMethod("rgb", ["r", "g", "b"], (vm, stack, target, arity, loc) =>
            {
                int b = stack.Pop().CastUnbox(Int);
                int g = stack.Pop().CastUnbox(Int);
                int r = stack.Pop().CastUnbox(Int);

                stack.Push(Color, System.Drawing.Color.FromArgb(255, r, g, b));
            });

        BindMethod("say", [], (vm, stack, target, arity, loc) =>
            {
                stack.Reverse(arity);
                var res = new StringBuilder();

                while (arity > 0)
                {
                    stack.Pop().Say(vm, res);
                    arity--;
                }

                Console.WriteLine(res.ToString());
            });

        BindMacro("set", ["id1", "value1", "id2?", "value2?"], (vm, target, args, loc) =>
        {
            while (!args.Empty)
            {
                var id = args.Pop().Cast<Forms::Id>();
                var v = args.Pop();
                var b = vm.Env[id.Name];
                if (b is null) { throw new EmitError($"Unknown symbol: {id.Name}", loc); }
                vm.Emit(v);
                vm.Emit(Ops.SetRegister.Make(((Value)b).CastUnbox(Binding)));
            }
        });

        BindMacro("spawn", ["args", "body?"], (vm, target, args, loc) =>
        {
            var forkArgs = args.Pop().Cast<Forms::Array>().Items;
            if (forkArgs.Length != 1) { throw new EmitError("Wrong number of arguments.", loc); }

            var c1 = PipeType.Make();
            var c2 = PipeType.Make();

            var fvm = new VM(vm.Config);
            var portName = forkArgs[0].Cast<Forms::Id>().Name;
            fvm.Env.Bind(portName, Value.Make(Port, new PipePort(c1.Reader, c2.Writer)));
            var startPC = fvm.EmitPC;
            args.Emit(fvm);
            fvm.Emit(Ops.Stop.Make());

            vm.Emit(Ops.Push.Make(Value.Make(Port, new PipePort(c2.Reader, c1.Writer))));
            new Thread(() => fvm.Eval(startPC)).Start();
        });

        BindMacro("stop", [], (vm, target, args, loc) => vm.Emit(Ops.Stop.Make()));

        BindMethod("type-of", ["x"], (vm, stack, target, arity, loc) =>
            stack.Push(Meta, stack.Pop().Type));

        BindMacro("var", ["id", "value"], (vm, target, args, loc) =>
            {
                while (true)
                {
                    var id = args.TryPop();
                    if (id is null) { break; }

                    if (args.TryPop() is Form f)
                    {
                        vm.Emit(f);
                        vm.Define(id);
                    }
                    else { throw new EmitError("Missing value", loc); }
                }
            });
    }

    private static void bindId(VM vm, Forms.Id idf, List<(int, int, int)> brs)
    {
        if (vm.Env.Find(idf.Name) is Value v && v.Type == Binding)
        {
            var r = vm.AllocRegister();
            var b = v.CastUnbox(Binding);
            brs.Add((r, b.FrameOffset, b.Index));
            vm.Emit(Ops.CopyRegister.Make(b, new Register(0, r)));
            vm.Emit(Ops.SetRegister.Make(b));
        }
        else
        {
            var r = new Register(0, vm.AllocRegister());
            vm.Emit(Ops.SetRegister.Make(r));
            vm.Env[idf.Name] = Value.Make(Binding, r);
        }
    }

    private static void bind(VM vm, Form f, List<(int, int, int)> brs)
    {
        switch (f)
        {
            case Forms.Id idf:
                bindId(vm, idf, brs);
                break;
            case Forms.Nil:
                vm.Emit(Ops.Drop.Make(1));
                break;
            case Forms.Pair pf:
                vm.Emit(Ops.Unzip.Make(pf.Loc));
                vm.Emit(Ops.Swap.Make(pf.Loc));
                bind(vm, pf.Left, brs);
                bind(vm, pf.Right, brs);
                break;
            default:
                throw new EmitError($"Invalid lvalue: {f}", f.Loc);
        }
    }
}