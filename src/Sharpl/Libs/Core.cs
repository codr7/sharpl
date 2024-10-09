using Sharpl.Types.Core;
using System.Text;
using Forms = Sharpl.Forms;

namespace Sharpl.Libs;

public class Core : Lib
{
    public static readonly BasicType Any = new BasicType("Any", []);
    public static readonly ArrayType Array = new ArrayType("Array", [Any]);
    public static readonly BindingType Binding = new BindingType("Binding", [Any]);
    public static readonly BitType Bit = new BitType("Bit", [Any]);
    public static readonly CharType Char = new CharType("Char", [Any]);
    public static readonly ColorType Color = new ColorType("Color", [Any]);
    public static readonly DurationType Duration = new DurationType("Duration", [Any]);
    public static readonly ErrorType Error = new ErrorType("Error", [Any]);
    public static readonly FixType Fix = new FixType("Fix", [Any]);
    public static readonly FormType Form = new FormType("Form", [Any]);
    public static readonly IntType Int = new IntType("Int", [Any]);
    public static readonly IterType Iter = new IterType("Iter", [Any]);
    public static readonly LibType Lib = new LibType("Lib", [Any]);
    public static readonly ListType List = new ListType("List", [Any]);
    public static readonly LocType Loc = new LocType("Loc", [Any]);
    public static readonly MacroType Macro = new MacroType("Macro", [Any]);
    public static readonly MapType Map = new MapType("Map", [Any]);
    public static readonly MetaType Meta = new MetaType("Meta", [Any]);
    public static readonly MethodType Method = new MethodType("Method", [Any]);
    public static readonly NilType Nil = new NilType("Nil", []);
    public static readonly PairType Pair = new PairType("Pair", [Any]);
    public static readonly PipeType Pipe = new PipeType("Pipe", [Any]);
    public static readonly PortType Port = new PortType("Port", [Any]);
    public static readonly StringType String = new StringType("String", [Any]);
    public static readonly SymType Sym = new SymType("Sym", [Any]);
    public static readonly TimestampType Timestamp = new TimestampType("Timestamp", [Any]);
    public static readonly UserMethodType UserMethod = new UserMethodType("UserMethod", [Any]);

    public static void DefineMethod(Loc loc, VM vm, Form.Queue args, Register result, Type<UserMethod> type, Op stopOp)
    {
        var name = "";
        var f = args.TryPop();

        if (f is Forms.Id id)
        {
            name = id.Name;
            f = args.TryPop();
        }

        var fas = new List<UserMethod.Arg>();
        var parentEnv = vm.Env;
        var registerCount = vm.NextRegisterIndex;

        var ids = args.CollectIds().Where(id =>
           vm.Env[id] is Value v &&
           v.Type == Binding &&
           v.CastUnbox(Binding).FrameOffset != -1).
           ToHashSet();

        bool vararg = false;

        vm.DoEnv(new Env(vm.Env, ids), loc, () =>
        {
            if (f is Forms.Array af)
            {
                for (var i = 0; i < af.Items.Length; i++)
                {
                    var f = af.Items[i];
                    if (vararg) { throw new EmitError("Vararg must be final param.", f.Loc); }

                    if (f is Forms.Splat s)
                    {
                        f = s.Target;
                        vararg = true;
                    }

                    Sharpl.UserMethod.Bind(vm, f, fas, false);
                }
            }
            else { throw new EmitError($"Invalid method args: {f!.Dump(vm)}", loc); }

            var m = new UserMethod(vm, name, ids.ToArray(), fas.ToArray(), vararg, loc);
            var skip = new Label();
            if (ids.Count > 0) { vm.Emit(Ops.PrepareClosure.Make(m, skip)); }
            else { vm.Emit(Ops.Goto.Make(skip)); }
            m.StartPC = vm.EmitPC;
            var v = Value.Make(type, m);
            if (name != "") { parentEnv.Bind(name, v); }
            args.Emit(vm, vm.Result);
            m.EndPC = vm.EmitPC;
            vm.Emit(stopOp);
            skip.PC = vm.EmitPC;
            if (name == "") { v.Emit(vm, args, result, loc); }
        });
    }

    public readonly Register LOC;

    public Core(VM vm) : base("core", null, [])
    {
        BindType(Any);
        BindType(Array);
        BindType(Binding);
        BindType(Bit);
        BindType(Char);
        BindType(Color);
        BindType(Duration);
        BindType(Error);
        BindType(Fix);
        BindType(Int);
        BindType(Lib);
        BindType(Loc);
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

        LOC = vm.AllocVar();

        BindMacro("^", [], (vm, target, args, result, loc) =>
            DefineMethod(loc, vm, args, result, UserMethod, Ops.ExitMethod.Make()));

        BindMethod("=", ["x"], (vm, target, arity, result, loc) =>
        {
            var v = vm.GetRegister(0, 0);
            var res = true;

            for (var i = 1; i < arity; i++)
            {
                if (!vm.GetRegister(0, i).Equals(v))
                {
                    res = false;
                    break;
                }

                arity--;
            }

            vm.Set(result, Value.Make(Bit, res));
        });

        BindMethod("<", ["x", "y"], (vm, target, arity, result, loc) =>
        {
            var lv = vm.GetRegister(0, 0);
            var res = true;
            var t = lv.Type.Cast<ComparableTrait>();
            if (t is null) throw new EvalError($"Not comparable: {lv}", loc);

            for (var i = 1; i < arity; i++)
            {
                var rv = vm.GetRegister(0, i);
                if (rv.Type != t) throw new EvalError($"Type mismatch: {lv} {rv}", loc);

                if (t.Compare(lv, rv) != Order.LT)
                {
                    res = false;
                    break;
                }

                lv = rv;
            }

            vm.Set(result, Value.Make(Bit, res));
        });

        BindMethod(">", ["x", "y"], (vm, target, arity, result, loc) =>
        {
            var lv = vm.GetRegister(0, 0);
            var res = true;
            var t = lv.Type.Cast<ComparableTrait>();
            if (t is null) { throw new EvalError($"Not comparable: {lv}", loc); }

            for (var i = 1; i < arity; i++)
            {
                var rv = vm.GetRegister(0, i);
                if (rv.Type != t) { throw new EvalError($"Type mismatch: {lv} {rv}", loc); }

                if (t.Compare(lv, rv) != Order.GT)
                {
                    res = false;
                    break;
                }

                lv = rv;
            }

            vm.Set(result, Value.Make(Bit, res));
        });


        BindMethod("+", ["x"], (vm, target, arity, result, loc) =>
        {
            var v = vm.GetRegister(0, 0);
            if (v.Type.Cast<NumericTrait>() is NumericTrait nt) { nt.Add(vm, arity, result, loc); }
            else { throw new NonNumericError(vm, v, vm.PC - 1, 0, loc); }
        });

        BindMethod("-", ["x"], (vm, target, arity, result, loc) =>
        {
            var v = vm.GetRegister(0, 0);
            if (v.Type.Cast<NumericTrait>() is NumericTrait nt) { nt.Subtract(vm, arity, result, loc); }
            else { throw new NonNumericError(vm, v, vm.PC - 1, 0, loc); }
        });

        BindMethod("*", ["x", "y"], (vm, target, arity, result, loc) =>
         {
             var v = vm.GetRegister(0, 0);
             if (v.Type.Cast<NumericTrait>() is NumericTrait nt) { nt.Multiply(vm, arity, result, loc); }
             else { throw new NonNumericError(vm, v, vm.PC - 1, 0, loc); }
         });

        BindMethod("/", ["x", "y"], (vm, target, arity, result, loc) =>
        {
            var v = vm.GetRegister(0, 0);
            if (v.Type.Cast<NumericTrait>() is NumericTrait nt) { nt.Divide(vm, arity, result, loc); }
            else { throw new NonNumericError(vm, v, vm.PC - 1, 0, loc); }
        });

        BindMacro("and", ["value1"], (vm, target, args, result, loc) =>
             {
                 var done = new Label();
                 var first = true;

                 while (!args.Empty)
                 {
                     if (!first) { vm.Emit(Ops.Drop.Make(1)); }
                     vm.Emit(args.Pop(), vm.Result);

                     if (!args.Empty)
                     {
                         vm.Emit(Ops.And.Make(vm.Result, done));
                         first = false;
                     }
                 }

                 done.PC = vm.EmitPC;
             });

        BindMacro("bench", ["n"], (vm, target, args, result, loc) =>
         {
             var n = vm.Eval(args.Pop());
             vm.Emit(Ops.Benchmark.Make(((Value)n!).CastUnbox(Int, loc), result));
             args.Emit(vm, vm.Result);
             vm.Emit(Ops.Stop.Make());
         });

        BindMacro("check", ["x"], (vm, target, args, result, loc) =>
         {
             var ef = args.Pop();
             var emptyArgs = true;
             var actual = new Register(0, 0);
             var expected = new Register(0, 1);

             if (!args.Empty)
             {
                 vm.Emit(Ops.BeginFrame.Make(vm.NextRegisterIndex));

                 vm.DoEnv(new Env(vm.Env, args.CollectIds()), loc, () =>
                 {
                     while (true)
                     {
                         if (args.TryPop() is Form bf) { vm.Emit(bf, actual); }
                         else { break; }
                     }

                 });

                 vm.Emit(Ops.EndFrame.Make(loc));
                 emptyArgs = false;
             }

             vm.Emit(ef, expected);
             if (emptyArgs) { Value.T.Emit(vm, new Form.Queue(), actual, loc); }
             vm.Emit(Ops.Check.Make(actual, expected, loc));
         });

        BindMethod("close", ["it"], (vm, target, arity, result, loc) =>
        {
            var it = vm.GetRegister(0, 0);
            if (it.Type.Cast<CloseTrait>() is CloseTrait ct) { ct.Close(it); }
            else { throw new EvalError($"Not supported: {it}", loc); }
        });

        BindMacro("data", ["name", "supers?", "cons?"], (vm, target, args, result, loc) =>
        {
            var n = args.Pop().Cast<Forms.Id>().Name;

            var sfs = args.Empty ? [] : args.Pop() switch
            {
                Forms.Array af => af.Items,
                Forms.Id idf => [idf],
                Forms.Nil => [],
                Form df => throw new EmitError("Invalid type definition: {df}", df.Loc)
            };

            var sts = new List<AnyType>();

            foreach (var sf in sfs)
            {
                if (sf.GetValue(vm) is Value sv) { sts.Add(sv.Cast(Meta, sf.Loc)); }
                else throw new EmitError($"Invalid super type: {sf.Dump(vm)}", sf.Loc);
            }

            if (sts.All(st => st is UserTrait)) { throw new EmitError("No concrete super type found", loc); }

            var c = args.Empty
                ? Value.Make(Method, new Method(n, ["value"], (vm, target, arity, result, loc) =>
                {
                    if (arity == 0) vm.SetRegister(0, 0, Value._);
                }))
                : vm.Eval(args.Pop());

            var t = new UserType(n, sts.ToArray(), (Value)c!);
            vm.Env[n] = Value.Make(Meta, t);
        });

        BindMacro("dec", ["delta?"], (vm, target, args, result, loc) =>
        {
            if (args.TryPop() is Forms.Id id && vm.Env[id.Name] is Value v && v.Type == Binding)
            {
                var r = v.CastUnbox(Binding);
                Value? d = args.Empty ? null : vm.Eval(args.Pop());
                vm.Emit(Ops.Decrement.Make(r, (d is null) ? 1 : d!.Value.CastUnbox(Int, loc)));
                vm.Emit(Ops.GetRegister.Make(r));
            }
            else { throw new EmitError("Invalid target", loc); }
        });

        BindMethod("defer", ["target1", "target2?"], (vm, target, arity, result, loc) =>
        {
            for (var i = 0; i < arity; i++) { vm.Defer(vm.GetRegister(0, i)); }
        });

        BindMacro("dmit", [], (vm, target, args, result, loc) =>
        {
            var skip = new Label();
            vm.Emit(Ops.Goto.Make(skip));
            var startPC = vm.EmitPC;
            foreach (var a in args) { vm.Emit(a.Unquote(vm, loc), vm.Result); }
            args.Clear();
            skip.PC = vm.EmitPC;
            vm.Emit(Ops.Stop.Make());
            vm.Dmit(startPC);
        });

        BindMacro("do", [], (vm, target, args, result, loc) =>
        {
            vm.Emit(Ops.BeginFrame.Make(vm.NextRegisterIndex));
            vm.DoEnv(new Env(vm.Env, args.CollectIds()), loc, () => args.Emit(vm, result));
            vm.Emit(Ops.EndFrame.Make(loc));
        });

        BindMethod("dump", [], (vm, target, arity, result, loc) =>
        {
            var res = new StringBuilder();

            for (var i = 0; i < arity; i++)
            {
                if (i > 0) res.Append(' ');
                vm.GetRegister(0, i).Dump(vm, res);
            }

            Console.WriteLine(res.ToString());
        });

        BindMacro("else", ["condition", "true?", "false?"], (vm, target, args, result, loc) =>
        {
            if (args.TryPop() is Form cf) vm.Emit(cf, result);
            else throw new EmitError("Missing condition", loc);
            var skipElse = new Label();
            vm.Emit(Ops.Branch.Make(result, skipElse, loc));
            if (args.TryPop() is Form tf) vm.Emit(tf, result);
            var skipEnd = new Label();
            vm.Emit(Ops.Goto.Make(skipEnd));
            skipElse.PC = vm.EmitPC;
            args.Emit(vm, result);
            skipEnd.PC = vm.EmitPC;
        });

        BindMacro("emit", ["code?"], (vm, target, args, result, loc) =>
        {
            var skip = new Label();
            vm.Emit(Ops.Goto.Make(skip));
            var startPC = vm.EmitPC;
            foreach (var a in args) { vm.Emit(a.Unquote(vm, loc), result); }
            args.Clear();
            vm.Emit(Ops.Stop.Make());
            skip.PC = vm.EmitPC;
            vm.Eval(startPC);
            vm.Emit(Ops.SetRegisterDirect.Make(result, vm.Get(result)));
        });

        BindMethod("eval", ["code"], (vm, target, arity, result, loc) =>
        {
            var v = vm.GetRegister(0, 0);

            if (v.Type == String)
            {
                var rv = vm.Eval(v.Cast(String));
                vm.Set(result, rv ?? Value._);
            }
            else
            {
                var f = v.Unquote(vm, loc);
                vm.Eval(f, result);
            }
        });

        BindMethod("exit", ["code?"], (vm, target, arity, result, loc) =>
            Environment.Exit((arity == 0) ? 0 : vm.GetRegister(0, 0).CastUnbox(Int)));

        BindMethod("fail", ["type", "arg"], (vm, target, arity, result, loc) =>
        {
            var tv = vm.GetRegister(0, 0);
            var t = (tv == Value._) ? Error : tv.Cast(Meta, loc);
            for (int i = 0; i < arity - 1; i++) { vm.SetRegister(0, i, vm.GetRegister(0, i + 1)); }
            t.Call(vm, arity, vm.Result, loc);
            throw new UserError(vm, vm.Get(vm.Result), loc);
        });

        BindMethod("gensym", ["name"], (vm, target, arity, result, loc) =>
            {
                var res = new StringBuilder();
                for (var i = 0; i < arity; i++) vm.GetRegister(0, i).Say(vm, res);
                vm.Set(result, Value.Make(Sym, vm.Gensym(res.ToString())));
            });

        BindMacro("if", ["condition"], (vm, target, args, result, loc) =>
            {
                var cond = new Register(0, vm.AllocRegister());
                if (args.TryPop() is Form f) { vm.Emit(f, cond); }
                else { throw new EmitError("Missing condition", loc); }
                var skip = new Label();
                vm.Emit(Ops.Branch.Make(cond, skip, loc));
                args.Emit(vm, result);
                skip.PC = vm.EmitPC;
            });

        BindMacro("inc", ["delta?"], (vm, target, args, result, loc) =>
        {
            if (args.TryPop() is Forms.Id id && vm.Env[id.Name] is Value v && v.Type == Binding)
            {
                var r = v.CastUnbox(Binding);
                Value? d = args.Empty ? null : vm.Eval(args.Pop());
                vm.Emit(Ops.Increment.Make(r, (d is null) ? 1 : d!.Value.CastUnbox(Int, loc)));
                vm.Emit(Ops.GetRegister.Make(r));
            }
            else { throw new EmitError("Invalid target", loc); }
        });

        BindMethod("is", ["x"], (vm, target, arity, result, loc) =>
        {
            var v = vm.GetRegister(0, 0);
            var res = true;

            for (var i = 1; i < arity; i++)
            {
                var sv = vm.GetRegister(0, 1);

                if (sv.Type != v.Type || !sv.Data.Equals(v.Data))
                {
                    res = false;
                    break;
                }
            }

            vm.Set(result, Value.Make(Bit, res));
        });

        BindMethod("isa", ["x", "y"], (vm, target, arity, result, loc) =>
            vm.Set(result, Value.Make(Bit, vm.GetRegister(0, 0).Isa(vm.GetRegister(0, 1).Cast(Meta, loc)))));

        BindMethod("length", ["it"], (vm, target, arity, result, loc) =>
            {
                var v = vm.GetRegister(0, 0);
                if (v.Type.Cast<LengthTrait>() is LengthTrait st) { vm.Set(result, Value.Make(Int, st.Length(v))); }
                else { throw new EvalError($"Not supported: {v}", loc); }
            });

        BindMacro("let", ["bindings"], (vm, target, args, result, loc) =>
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

                    vm.DoEnv(new Env(vm.Env, ids), loc, () =>
                     {
                         var brs = new List<(int, int, int)>();

                         for (var i = 0; i < bs.Length; i++)
                         {
                             var f = bs[i];
                             i++;
                             vm.Emit(bs[i], vm.Result);
                             bind(vm, f, vm.Result, brs);
                         }

                         while (true)
                         {
                             if (args.TryPop() is Form f) { vm.Emit(f, result); }
                             else { break; }
                         }

                         foreach (var (fromIndex, toFrameOffset, toIndex) in brs)
                         {
                             vm.Emit(Ops.CopyRegister.Make(
                                 new Register(0, fromIndex), 
                                 new Register(toFrameOffset, toIndex)));
                         }

                         vm.Emit(Ops.EndFrame.Make(loc));
                     });
                }
                else { throw new EmitError("Missing bindings", loc); }
            });

        BindMacro("lib", [], (vm, target, args, result, loc) =>
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
                else { vm.DoEnv(lib, loc, () => args.Emit(vm, result)); }
            }
            else { throw new EmitError("Invalid library name", loc); }
        });

        BindMacro("load", ["path"], (vm, target, args, result, loc) =>
        {
            while (true)
            {
                if (args.TryPop() is Form pf)
                {
                    if (vm.Eval(pf) is Value p) vm.Load(p.Cast(String, pf.Loc));
                    else throw new EvalError("Missing path", pf.Loc);
                }
                else break;
            }
        });

        BindMacro("loop", ["body?"], (vm, target, args, result, loc) =>
        {
            vm.Emit(Ops.BeginFrame.Make(vm.NextRegisterIndex));

            vm.DoEnv(new Env(vm.Env, args.CollectIds()), loc, () =>
             {
                 var end = new Label();
                 var start = new Label(vm.EmitPC);
                 args.Emit(vm, result);
                 vm.Emit(Ops.Goto.Make(start));
                 end.PC = vm.EmitPC;
                 vm.Emit(Ops.EndFrame.Make(loc));
             });
        });

        BindMethod("max", ["x", "y?"], (vm, target, arity, result, loc) =>
        {
            var v = vm.GetRegister(0, 0);
            var t = v.Type.Cast<ComparableTrait>();

            for (var i = 1; i  < arity; i++)
            {
                var sv = vm.GetRegister(0, i);
                if (sv.Type != t) { throw new EvalError($"Type mismatch: {sv.Type}/{t}", loc); }
                if (t.Compare(sv, v) != Order.LT) { v = sv; }
            }

            vm.Set(result, v);
        });

        BindMethod("min", ["x", "y?"], (vm, target, arity, result, loc) =>
        {
            var v = vm.GetRegister(0, 0);
            var t = v.Type as ComparableTrait;
            arity--;

            while (arity > 0)
            {
                var sv = vm.GetRegister(0, 1);
                if (sv.Type != t) { throw new EvalError("Wrong type: {sv.Type}/{t}", loc); }
                if (t.Compare(sv, v) != Order.GT) { v = sv; }
                arity--;
            }

            vm.Set(result, v);
        });

        BindMethod("not", ["it"], (vm, target, arity, result, loc) => 
            vm.Set(result, Value.Make(Bit, !(bool)vm.GetRegister(0, 0))));

        BindMacro("or", ["value1"], (vm, target, args, result, loc) =>
             {
                 var done = new Label();

                 while (!args.Empty)
                 {
                     vm.Emit(args.Pop(), result);
                     if (!args.Empty) vm.Emit(Ops.Or.Make(result, done));
                 }

                 done.PC = vm.EmitPC;
             });

        BindMethod("parse-int", ["val"], (vm, target, arity, result, loc) =>
        {
            int v = 0;
            var i = 0;

            foreach (char c in vm.GetRegister(0, 0).Cast(String, loc))
            {

                if (!char.IsDigit(c)) { break; }
                v = v * 10 + c - '0';
                i++;
            }

            vm.Set(result, Value.Make(Pair, (Value.Make(Int, v), Value.Make(Int, i))));
        });

        BindMethod("peek", ["src"], (vm, target, arity, result, loc) =>
        {
            var src = vm.GetRegister(0, 0);
            if (src.Type.Cast<StackTrait>() is StackTrait st) { vm.Set(result, st.Peek(loc, vm, src)); }
            else { throw new EvalError("Invalid peek target: {src}", loc); }
        });

        BindMethod("poll", ["sources"], (vm, target, arity, result, loc) =>
        {
            var ssv = vm.GetRegister(0, 0);

            if (ssv.Type.Cast<IterTrait>() is IterTrait it)
            {
                var ss = new List<(Task<bool>, Value)>();
                var cts = new CancellationTokenSource();

                for (var i = it.CreateIter(ssv, vm, loc); i.Next(vm, loc) is Value v;)
                {
                    if (v.Type.Cast<PollTrait>() is PollTrait pt) { ss.Add((pt.Poll(v, cts.Token), v)); }
                    else { throw new EvalError($"Not pollable: {v.Dump(vm)}", loc); }
                }

                vm.Set(result, Task.Run(async () => await TaskUtil.Any(ss.ToArray(), cts)).Result);
            }
            else { throw new EvalError($"Not iterable: {ssv.Dump(vm)}", loc); }
        });

        BindMacro("pop", ["src"], (vm, target, args, result, loc) =>
         {
             var src = args.Pop();

             if (src is Forms.Id id && vm.Env[id.Name] is Value v && v.Type == Binding)
             {
                 vm.Emit(Ops.PopItem.Make(v.CastUnbox(Binding), result, loc));
             }
             else { throw new EmitError($"Invalid push destination: {src}", loc); }
         });

        BindMacro("push", ["dst", "val"], (vm, target, args, result, loc) =>
         {
             var dst = args.Pop();

             if (dst is Forms.Id id && vm.Env[id.Name] is Value v && v.Type == Binding)
             {
                 var it = new Register(0, vm.AllocRegister());

                 foreach (var a in args)
                 {
                     vm.Emit(a, it);
                     vm.Emit(Ops.PushItem.Make(v.CastUnbox(Binding), it, loc));
                 }
             }
             else { throw new EmitError($"Invalid push destination: {dst}", loc); }
         });

        BindMethod("rand-int", ["n?"], (vm, target, arity, result, loc) =>
        {
            Value? n = (arity == 1) ? vm.GetRegister(0, 0) : null;
            var v = vm.Random.Next();
            vm.Set(result, Value.Make(Int, (n is null) ? v : v % ((Value)n).CastUnbox(Int, loc)));
        });

        BindMethod("range", ["max", "min?", "stride?"], (vm, target, arity, result, loc) =>
            {
                Value max = Value._, min = Value._, stride = Value._;
                AnyType t = Nil;

                switch (arity)
                {
                    case 1:
                        min = vm.GetRegister(0, 0);
                        t = (min.Type == Nil || t != Nil) ? t : min.Type;
                        break;
                    case 2:
                        max = vm.GetRegister(0, 1);
                        t = (max.Type == Nil) ? t : max.Type;
                        goto case 1;
                    case 3:
                        stride = vm.GetRegister(0, 2);
                        t = (stride.Type == Nil || t != Nil) ? t : stride.Type;
                        goto case 2;
                }

                if (t.Cast<RangeTrait>() is RangeTrait rt) vm.Set(result, Value.Make(Iter, rt.CreateRange(min, max, stride, loc)));
                else throw new EvalError($"Invalid range type: {t}", loc);
            });

        BindMethod("resize", ["array", "size", "value?"], (vm, target, arity, result, loc) =>
        {
            var v = (arity == 3) ? vm.GetRegister(0, 2) : Value._;
            var s = vm.GetRegister(0, 1).CastUnbox(Int, loc);
            var a = vm.GetRegister(0, 0).Cast(Array, loc);
            var ps = a.Length;
            System.Array.Resize(ref a, s);
            for (var i = ps; i < s; i++) { a[i] = v; }
            vm.Set(result, Value.Make(Array, a));
        });

        BindMacro("return", [], (vm, target, args, result, loc) =>
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
                            if (f.GetValue(vm) is Value v && 
                                (v.Type != Binding || v.CastUnbox(Binding).FrameOffset != 0)) { argMask[i] = v; }
                            else { vm.Emit(f, new Register(0, i)); }
                            i++;
                        }

                        vm.Emit(Ops.CallTail.Make(m, argMask, splat, result, loc));
                    }
                }

                if (m is null)
                {
                    if (vf is not null) { vm.Emit(vf, result); }
                    vm.Emit(Ops.ExitMethod.Make());
                }
            });

        BindMethod("rgb", ["r", "g", "b"], (vm, target, arity, result, loc) =>
            {
                int r = vm.GetRegister(0, 0).CastUnbox(Int);
                int g = vm.GetRegister(0, 1).CastUnbox(Int);
                int b = vm.GetRegister(0, 2).CastUnbox(Int);
                vm.Set(result, Value.Make(Color, System.Drawing.Color.FromArgb(255, r, g, b)));
            });

        BindMethod("say", [], (vm, target, arity, result, loc) =>
            {
                var res = new StringBuilder();
                for (var i = 0; i < arity; i++) vm.GetRegister(0, i).Say(vm, res);
                Console.WriteLine(res.ToString());
            });

        BindMacro("set", ["id1", "value1", "id2?", "value2?"], (vm, target, args, result, loc) =>
        {
            while (!args.Empty)
            {
                var id = args.Pop().Cast<Forms::Id>();
                var v = args.Pop();
                var b = vm.Env[id.Name];
                if (b is null) { throw new EmitError($"Unknown symbol: {id.Name}", loc); }
                vm.Emit(v, result);
                vm.Emit(Ops.SetRegister.Make(((Value)b).CastUnbox(Binding), result));
            }
        });

        BindMacro("spawn", ["args", "body?"], (vm, target, args, result, loc) =>
        {
            var forkArgs = args.Pop().Cast<Forms::Array>().Items;
            if (forkArgs.Length != 1) { throw new EmitError("Wrong number of arguments.", loc); }

            var c1 = PipeType.Make();
            var c2 = PipeType.Make();

            var fvm = new VM(vm.Config);
            var portName = forkArgs[0].Cast<Forms::Id>().Name;
            fvm.Env.Bind(portName, Value.Make(Port, new PipePort(c1.Reader, c2.Writer)));
            var startPC = fvm.EmitPC;
            args.Emit(fvm, fvm.Result);
            fvm.Emit(Ops.Stop.Make());

            vm.Emit(Ops.Push.Make(Value.Make(Port, new PipePort(c2.Reader, c1.Writer))));
            new Thread(() => fvm.Eval(startPC)).Start();
        });

        BindMacro("stop", [], (vm, target, args, result, loc) => vm.Emit(Ops.Stop.Make()));

        BindMacro("try", ["handlers", "body?"], (vm, target, args, result, loc) =>
        {
            vm.DoEnv(vm.Env, loc, () =>
            {
                vm.Env.Bind("LOC", Value.Make(Binding, LOC));
                var hsf = args.Pop();
                var hs = vm.Eval(hsf);
                var end = new Label();

                if (hs is Value hsv)
                {
                    vm.Emit(Ops.Try.Make(hsv.Cast(Array, loc).
                              Select(it => it.CastUnbox(Pair, loc)).
                              ToArray(), 
                            vm.NextRegisterIndex, end, LOC, loc));

                    args.Emit(vm, result);
                    vm.Emit(Ops.EndFrame.Make(loc));
                    end.PC = vm.EmitPC;
                }
                else { throw new EmitError($"Expected map literal: {hsf.Dump(vm)}", loc); }
            });
        });

        BindMacro("trait", ["name", "supers?"], (vm, target, args, result, loc) =>
        {
            var n = args.Pop().Cast<Forms.Id>().Name;

            var sfs = args.Empty ? [] : args.Pop() switch
            {
                Forms.Nil => [],
                Forms.Id idf => [idf],
                Forms.Array af => af.Items,
                Form df => throw new EmitError("Invalid trait definition: {df}", df.Loc)
            };

            var sts = new List<UserTrait>();

            foreach (var sf in sfs)
            {
                if (sf.GetValue(vm) is Value sv) { sts.Add((UserTrait)sv.Cast(Meta, sf.Loc)); }
                else throw new EmitError($"Invalid super trait: {sf.Dump(vm)}", sf.Loc);
            }

            var t = new UserTrait(n, sts.ToArray());
            vm.Env[n] = Value.Make(Meta, t);
        });

        BindMethod("type-of", ["x"], (vm, target, arity, result, loc) =>
            vm.Set(result, Value.Make(Meta, vm.GetRegister(0, 0).Type)));

        BindMacro("var", ["id", "value"], (vm, target, args, result, loc) =>
        {
            while (true)
            {
                var id = args.TryPop();
                if (id is null) { break; }

                if (args.TryPop() is Form f)
                {
                    vm.Emit(f, result);
                    vm.BindVar(id, result);
                }
                else { throw new EmitError("Missing value", loc); }
            }
        });
    }

    private static void bindId(VM vm, Forms.Id idf, Register value, List<(int, int, int)> brs)
    {
        if (vm.Env.Find(idf.Name) is Value v && v.Type == Binding)
        {
            var r = new Register(0, vm.AllocRegister());
            var b = v.CastUnbox(Binding);
            brs.Add((r.Index, b.FrameOffset, b.Index));
            vm.Emit(Ops.CopyRegister.Make(b, r));
            vm.Emit(Ops.SetRegister.Make(b, value));
        }
        else
        {
            var r = new Register(0, vm.AllocRegister());
            vm.Emit(Ops.SetRegister.Make(r, value));
            vm.Env[idf.Name] = Value.Make(Binding, r);
        }
    }

    private static void bind(VM vm, Form f, Register value, List<(int, int, int)> brs)
    {
        switch (f)
        {
            case Forms.Id idf:
                bindId(vm, idf, value, brs);
                break;
            case Forms.Nil:
                vm.Emit(Ops.Drop.Make(1));
                break;
            case Forms.Pair pf:
                
                vm.Emit(Ops.Unzip.Make(value, value, vm.Result, f.Loc));
                bind(vm, pf.Left, value, brs);
                bind(vm, pf.Right, vm.Result, brs);
                break;
            default:
                throw new EmitError($"Invalid lvalue: {f}", f.Loc);
        }
    }
}