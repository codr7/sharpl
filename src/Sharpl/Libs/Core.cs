using Sharpl.Types.Core;
using System.Text;
using System.Text.RegularExpressions;

namespace Sharpl.Libs;

public class Core : Lib
{
    public static readonly ArrayType Array = new ArrayType("Array");
    public static readonly BindingType Binding = new BindingType("Binding");
    public static readonly BitType Bit = new BitType("Bit");
    public static readonly ColorType Color = new ColorType("Color");
    public static readonly FixType Fix = new FixType("Fix");
    public static readonly FormType Form = new FormType("Form");
    public static readonly IntType Int = new IntType("Int");
    public static readonly IterType Iter = new IterType("Iter");
    public static readonly LibType Lib = new LibType("Lib");
    public static readonly MacroType Macro = new MacroType("Macro");
    public static readonly MapType Map = new MapType("Map");
    public static readonly MetaType Meta = new MetaType("Meta");
    public static readonly MethodType Method = new MethodType("Method");
    public static readonly NilType Nil = new NilType("Nil");
    public static readonly PairType Pair = new PairType("Pair");
    public static readonly StringType String = new StringType("String");
    public static readonly SymType Sym = new SymType("Sym");
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
                    if (vararg) { throw new EmitError(f.Loc, "Vararg must be final param."); }

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

                    throw new EmitError(f.Loc, $"Invalid method arg: {f}");
                }).ToArray();
            }
            else { throw new EmitError(loc, "Invalid method args"); }

            var m = new UserMethod(loc, vm, name, ids.ToArray(), fas, vararg);
            if (ids.Count > 0) { vm.Emit(Ops.PrepareClosure.Make(m)); }
            var skip = new Label();
            vm.Emit(Ops.Goto.Make(skip));
            m.StartPC = vm.EmitPC;
            var v = Value.Make(type, m);
            if (name != "") { parentEnv.Bind(name, v); }
            args.Emit(vm, new Form.Queue());
            vm.Emit(stopOp);
            skip.PC = vm.EmitPC;
            if (name == "") { v.Emit(loc, vm, args); }
        });
    }

    public Core() : base("core", null, [])
    {
        BindType(Array);
        BindType(Binding);
        BindType(Bit);
        BindType(Color);
        BindType(Fix);
        BindType(Int);
        BindType(Lib);
        BindType(Macro);
        BindType(Map);
        BindType(Meta);
        BindType(Method);
        BindType(Pair);
        BindType(String);
        BindType(Sym);
        BindType(UserMethod);

        Bind("F", Value.F);
        Bind("_", Value.Nil);
        Bind("T", Value.T);

        BindMacro("^", [], (loc, target, vm, args) =>
            DefineMethod(loc, vm, args, UserMethod, Ops.ExitMethod.Make()));

        BindMethod("=", ["x"], (loc, target, vm, stack, arity) =>
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

        BindMethod("<", ["x", "y"], (loc, target, vm, stack, arity) =>
        {
            var lv = stack.Pop();
            arity--;
            var res = true;
            if (lv.Type is not ComparableTrait t) { throw new EvalError(loc, $"Not comparable: {lv}"); }

            while (arity > 0)
            {
                var rv = stack.Pop();
                if (rv.Type != t) { throw new EvalError(loc, $"Type mismatch: {lv} {rv}"); }

                if (t.Compare(lv, rv) != Order.GT)
                {
                    res = false;
                    break;
                }

                arity--;
            }

            stack.Push(Value.Make(Bit, res));
        });

        BindMethod(">", ["x", "y"], (loc, target, vm, stack, arity) =>
        {
            var lv = stack.Pop();
            arity--;
            var res = true;
            var t = lv.Type as ComparableTrait;
            if (t is null) { throw new EvalError(loc, $"Not comparable: {lv}"); }

            while (arity > 0)
            {
                var rv = stack.Pop();
                if (rv.Type != t) { throw new EvalError(loc, $"Type mismatch: {lv} {rv}"); }

                if (t.Compare(lv, rv) != Order.LT)
                {
                    res = false;
                    break;
                }

                arity--;
            }

            stack.Push(Value.Make(Bit, res));
        });


        BindMethod("+", [], (loc, target, vm, stack, arity) =>
        {
            if (stack.Peek().Type is NumericTrait nt) { nt.Add(loc, vm, stack, arity); }
            else { throw new EvalError(loc, $"Expected numeric value"); }
        });

        BindMethod("-", [], (loc, target, vm, stack, arity) =>
        {
            if (stack.Peek().Type is NumericTrait nt) { nt.Subtract(loc, vm, stack, arity); }
            else { throw new EvalError(loc, $"Expected numeric value"); }
        });

        BindMethod("*", ["x", "y"], (loc, target, vm, stack, arity) =>
         {
             if (stack.Peek().Type is NumericTrait nt)
             { nt.Multiply(loc, vm, stack, arity); }
             else { throw new EvalError(loc, $"Expected numeric value"); }
         });

        BindMethod("/", ["x", "y"], (loc, target, vm, stack, arity) =>
        {
            if (stack.Peek().Type is NumericTrait nt) { nt.Divide(loc, vm, stack, arity); }
            else { throw new EvalError(loc, $"Expected numeric value"); }
        });

        BindMacro("bench", ["n"], (loc, target, vm, args) =>
         {
             if (args.TryPop() is Form f && vm.Eval(f) is Value n)
             {
                 vm.Emit(Ops.Benchmark.Make(n.CastUnbox(loc, Int)));
                 args.Emit(vm, new Form.Queue());
                 vm.Emit(Ops.Stop.Make());
             }
             else { throw new EmitError(loc, "Missing repetitions"); }
         });

        BindMacro("check", ["x"], (loc, target, vm, args) =>
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
                         if (args.TryPop() is Form bf) { bf.Emit(vm, args); }
                         else { break; }
                     }

                 });

                 vm.Emit(Ops.EndFrame.Make());
                 emptyArgs = false;
             }

             vm.Emit(ef);
             if (emptyArgs) { Value.T.Emit(loc, vm, new Form.Queue()); }
             vm.Emit(Ops.Check.Make(loc));
         });

        BindMacro("dec", [], (loc, target, vm, args) =>
        {
            if (args.TryPop() is Forms.Id id)
            {
                if (vm.Env[id.Name] is Value v)
                {
                    if (v.Type == Binding)
                    {
                        var b = v.CastUnbox(Binding);
                        vm.Emit(Ops.Decrement.Make(b.FrameOffset, b.Index));
                    }
                }
                else { throw new EmitError(id.Loc, "Invalid target"); }
            }
            else { throw new EmitError(loc, "Invalid target"); }
        });

        BindMacro("demit", [], (loc, target, vm, args) =>
        {
            var skip = new Label();
            vm.Emit(Ops.Goto.Make(skip));
            var startPC = vm.EmitPC;
            foreach (var a in args) { vm.Emit(a.Unquote(loc, vm)); }
            args.Clear();
            skip.PC = vm.EmitPC;
            vm.Emit(Ops.Stop.Make());
            vm.Demit(startPC);
        });

        BindMacro("do", [], (loc, target, vm, args) =>
        {
            vm.Emit(Ops.BeginFrame.Make(vm.NextRegisterIndex));
            vm.DoEnv(new Env(vm.Env, args.CollectIds()), () => args.Emit(vm, new Form.Queue()));
            vm.Emit(Ops.EndFrame.Make());
        });

        BindMacro("eval", [], (loc, target, vm, args) =>
        {
            var skip = new Label();
            vm.Emit(Ops.Goto.Make(skip));
            var startPC = vm.EmitPC;
            foreach (var a in args) { vm.Emit(a.Unquote(loc, vm)); }
            args.Clear();
            skip.PC = vm.EmitPC;
            vm.Emit(Ops.Stop.Make());

            var stack = new Stack();
            vm.Eval(startPC, stack);
            stack.Reverse();
            foreach (var it in stack) { args.PushFirst(new Forms.Literal(loc, it)); }
        });

        BindMethod("fail", [], (loc, target, vm, stack, arity) =>
        {
            stack.Reverse(arity);
            var res = new StringBuilder();

            while (arity > 0)
            {
                stack.Pop().Say(res);
                arity--;
            }

            throw new EvalError(loc, res.ToString());
        });

        BindMacro("for", ["vars", "body?"], (loc, target, vm, args) =>
        {
            vm.Emit(Ops.BeginFrame.Make(vm.NextRegisterIndex));

            vm.DoEnv(new Env(vm.Env, args.CollectIds()), () =>
             {
                 var bindings = new List<(int, int)>();

                 if (args.Pop() is Forms.Array vfs)
                 {
                     for (var i = 0; i < vfs.Items.Length; i += 2)
                     {
                         var idForm = vfs.Items[i];
                         var valForm = vfs.Items[i + 1];
                         var seqReg = vm.AllocRegister();
                         vm.Emit(valForm);
                         vm.Emit(Ops.CreateIter.Make(loc, new Register(0, seqReg)));
                         var itReg = vm.AllocRegister();
                         bindings.Add((seqReg, itReg));
                         if (idForm is Forms.Id idf) { vm.Env.Bind(idf.Name, Value.Make(Binding, new Register(0, itReg))); }
                         else { throw new EmitError(loc, "Expected id: " + idForm); }
                     }
                 }
                 else { throw new EmitError(loc, "Invalid loop bindings"); }

                 var end = new Label();
                 var start = new Label(vm.EmitPC);
                 
                 foreach (var (seqReg, itReg) in bindings) {
                    vm.Emit(Ops.IterNext.Make(loc, new Register(0, seqReg), end));
                    vm.Emit(Ops.SetRegister.Make(0, itReg));
                 }

                 args.Emit(vm);
                 vm.Emit(Ops.Goto.Make(start));
                 end.PC = vm.EmitPC;
                 vm.Emit(Ops.EndFrame.Make());
             });
        });

        BindMacro("if", ["condition"], (loc, target, vm, args) =>
            {
                vm.Emit(Ops.BeginFrame.Make(vm.NextRegisterIndex));

                vm.DoEnv(new Env(vm.Env, args.CollectIds()), () =>
                 {
                     if (args.TryPop() is Form f) { f.Emit(vm, args); }
                     else { throw new EmitError(loc, "Missing condition"); }

                     var skip = new Label();
                     vm.Emit(Ops.Branch.Make(skip));
                     args.Emit(vm, new Form.Queue());
                     skip.PC = vm.EmitPC;
                     vm.Emit(Ops.EndFrame.Make());
                 });
            });

        BindMacro("else", ["condition", "true?", "false?"], (loc, target, vm, args) =>
             {
                 vm.Emit(Ops.BeginFrame.Make(vm.NextRegisterIndex));

                 vm.DoEnv(new Env(vm.Env, args.CollectIds()), () =>
                  {
                      if (args.TryPop() is Form cf) { cf.Emit(vm, args); }
                      else { throw new EmitError(loc, "Missing condition"); }
                      var skipElse = new Label();
                      vm.Emit(Ops.Branch.Make(skipElse));
                      if (args.TryPop() is Form tf) { tf.Emit(vm, args); }
                      var skipEnd = new Label();
                      vm.Emit(Ops.Goto.Make(skipEnd));
                      skipElse.PC = vm.EmitPC;
                      args.Emit(vm, new Form.Queue());
                      skipEnd.PC = vm.EmitPC;
                      vm.Emit(Ops.EndFrame.Make());
                  });
             });

        BindMethod("is", ["x"], (loc, target, vm, stack, arity) =>
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

        BindMethod("len", ["it"], (loc, target, vm, stack, arity) =>
            {
                var v = stack.Pop();

                if (v.Type is SeqTrait st) { stack.Push(Int, st.Length(v)); }
                else { throw new EvalError(loc, $"Expected sequence: {v}"); }
            });

        BindMacro("let", ["bindings"], (loc, target, vm, args) =>
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
                             var bf = bs[i];

                             if (bf is Forms.Id idf)
                             {
                                 i++;
                                 vm.Emit(bs[i]);

                                 if (vm.Env.Find(idf.Name) is Value v && v.Type == Binding)
                                 {
                                     var r = vm.AllocRegister();
                                     var b = v.CastUnbox(Binding);
                                     brs.Add((r, b.FrameOffset, b.Index));
                                     vm.Emit(Ops.CopyRegister.Make(b.FrameOffset, b.Index, 0, r));
                                     vm.Emit(Ops.SetRegister.Make(b.FrameOffset, b.Index));
                                 }
                                 else
                                 {
                                     var r = vm.AllocRegister();
                                     vm.Emit(Ops.SetRegister.Make(0, r));
                                     vm.Env[idf.Name] = Value.Make(Binding, new Register(0, r));
                                 }
                             }
                             else { throw new EmitError(bf.Loc, $"Invalid method arg: {bf}"); }
                         }

                         while (true)
                         {
                             if (args.TryPop() is Form f) { f.Emit(vm, args); }
                             else { break; }
                         }

                         foreach (var (fromIndex, toFrameOffset, toIndex) in brs)
                         {
                             vm.Emit(Ops.CopyRegister.Make(0, fromIndex, toFrameOffset, toIndex));
                         }

                         vm.Emit(Ops.EndFrame.Make());
                     });
                }
                else { throw new EmitError(loc, "Missing bindings"); }
            });

        BindMacro("lib", [], (loc, target, vm, args) =>
            {
                if (args.Count == 0) { vm.Emit(Ops.Push.Make(Value.Make(Lib, vm.Lib))); }

                else if (args.TryPop() is Forms.Id nf)
                {
                    Lib? lib = null;

                    if (vm.Env.Find(nf.Name) is Value v) { lib = v.Cast(loc, Lib); }
                    else
                    {
                        lib = new Lib(nf.Name, vm.Env, args.CollectIds());
                        vm.Env.BindLib(lib);
                    }

                    if (args.Empty) { vm.Env = lib; }
                    else { vm.DoEnv(lib, () => args.Emit(vm, new Form.Queue())); }
                }
                else { throw new EmitError(loc, "Invalid library name"); }
            });

        BindMacro("load", ["path"], (loc, target, vm, args) =>
            {
                while (true)
                {
                    if (args.TryPop() is Form pf)
                    {
                        if (vm.Eval(pf, args) is Value p) { vm.Load(p.Cast(pf.Loc, String)); }
                        else { throw new EvalError(pf.Loc, "Missing path"); }
                    }
                    else { break; }
                }
            });

        BindMethod("not", ["it"], (loc, target, vm, stack, arity) => stack.Push(Bit, !(bool)stack.Pop()));

        BindMacro("or", ["value1"], (loc, target, vm, args) =>
             {
                 var done = new Label();
                 var first = true;

                 while (!args.Empty)
                 {
                     if (!first) { vm.Emit(Ops.Drop.Make(1)); }
                     args.Pop().Emit(vm, args);

                     if (!args.Empty)
                     {
                         vm.Emit(Ops.Or.Make(done));
                         first = false;
                     }
                 }

                 done.PC = vm.EmitPC;
             });

        BindMethod("range", ["max", "min?", "stride?"], (loc, target, vm, stack, arity) =>
            {
                Value max = Value.Nil, min = Value.Nil, stride = Value.Nil;
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
                else { throw new EvalError(loc, $"Invalid range type: {t}"); }
            });

        BindMacro("return", [], (loc, target, vm, args) =>
            {
                UserMethod? m = null;

                if (args.TryPop() is Forms.Call c)
                {

                    if (c.Target is Forms.Literal lt && lt.Value is var lv && lv.Type == UserMethod) { m = lv.Cast(UserMethod); }
                    else if (c.Target is Forms.Id it && vm.Env[it.Name] is Value iv && iv.Type == UserMethod) { m = iv.Cast(UserMethod); }

                    if (m is UserMethod)
                    {
                        if (!args.Empty) { throw new EmitError(loc, "Too many args in tail call"); }
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

                        if (!splat && c.Args.Length < m.MinArgCount) { throw new EmitError(loc, $"Not enough arguments: {m}"); }
                        var argMask = new Value?[c.Args.Length];
                        var i = 0;

                        foreach (var f in c.Args)
                        {
                            /* Referring to the previous frame's bindings is tricky when we're not creating new frames.*/
                            if (f.GetValue(vm) is Value v && (v.Type != Binding || v.CastUnbox(Binding).FrameOffset != 1)) { argMask[i] = v; }
                            else { vm.Emit(f); }
                            i++;
                        }

                        vm.Emit(Ops.CallTail.Make(loc, m, argMask, splat));
                    }
                }

                if (m is null)
                {
                    args.Emit(vm, new Form.Queue());
                    vm.Emit(Ops.ExitMethod.Make());
                }
            });

        BindMacro("reduce", ["method", "sequence", "seed"], (loc, target, vm, args) =>
              {
                  var iter = new Register(0, vm.AllocRegister());
                  var methodForm = args.Pop();
                  var sequenceForm = args.Pop();
                  var seedForm = args.Pop();
                  var emptyArgs = new Form.Queue();

                  var method = new Register(0, vm.AllocRegister());
                  methodForm.Emit(vm, emptyArgs);
                  vm.Emit(Ops.SetRegister.Make(method.FrameOffset, method.Index));

                  sequenceForm.Emit(vm, emptyArgs);
                  vm.Emit(Ops.CreateIter.Make(loc, iter));
                  seedForm.Emit(vm, emptyArgs);

                  var start = new Label(vm.EmitPC);
                  var done = new Label();
                  vm.Emit(Ops.IterNext.Make(loc, iter, done));
                  vm.Emit(Ops.CallRegister.Make(loc, method, 2, false, vm.NextRegisterIndex));
                  vm.Emit(Ops.Goto.Make(start));
                  done.PC = vm.EmitPC;
              });

        BindMethod("rxplace", ["in", "old", "new"], (loc, target, vm, stack, arity) =>
            {
                var n = stack.Pop().Cast(loc, String);
                var o = stack.Pop().Cast(loc, String);
                var i = stack.Pop().Cast(loc, String);
                o = o.Replace(" ", "\\s*");
                stack.Push(Value.Make(String, Regex.Replace(i, o, n)));
            });

        BindMethod("rgb", ["r", "g", "b"], (loc, target, vm, stack, arity) =>
            {
                int b = stack.Pop().CastUnbox(Int);
                int g = stack.Pop().CastUnbox(Int);
                int r = stack.Pop().CastUnbox(Int);

                stack.Push(Color, System.Drawing.Color.FromArgb(255, r, g, b));
            });

        BindMethod("say", [], (loc, target, vm, stack, arity) =>
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

        BindMethod("sym", ["x"], (loc, target, vm, stack, arity) =>
            {
                stack.Reverse(arity);
                var res = new StringBuilder();

                while (arity > 0)
                {
                    stack.Pop().Say(res);
                    arity--;
                }

                stack.Push(Value.Make(Sym, vm.Intern(res.ToString())));
            });

        BindMacro("var", ["id", "value"], (loc, target, vm, args) =>
            {
                while (true)
                {
                    var id = args.TryPop();
                    if (id is null) { break; }

                    if (id is Forms.Id idf)
                    {
                        if (args.TryPop() is Form f)
                        {
                            if (f is Forms.Literal lit) { vm.Env[idf.Name] = lit.Value; }
                            var v = new Form.Queue();
                            v.Push(f);
                            v.Emit(vm, args);
                            vm.Define(idf.Name);
                        }
                        else { throw new EmitError(loc, "Missing value"); }
                    }
                    else { throw new EmitError(loc, $"Invalid binding: {id}"); }
                }
            });
    }
}