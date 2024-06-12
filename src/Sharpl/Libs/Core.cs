namespace Sharpl.Libs;

using Sharpl.Types.Core;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;


public class Core : Lib
{
    public static readonly ArrayType Array = new ArrayType("Array");
    public static readonly BindingType Binding = new BindingType("Binding");
    public static readonly BitType Bit = new BitType("Bit");
    public static readonly ColorType Color = new ColorType("Color");
    public static readonly IntType Int = new IntType("Int");
    public static readonly LibType Lib = new LibType("Lib");
    public static readonly MacroType Macro = new MacroType("Macro");
    public static readonly MetaType Meta = new MetaType("Meta");
    public static readonly MethodType Method = new MethodType("Method");
    public static readonly NilType Nil = new NilType("Nil");
    public static readonly StringType String = new StringType("String");
    public static readonly UserMethodType UserMethod = new UserMethodType("UserMethod");

    public Core() : base("core", null)
    {
        BindType(Array);
        BindType(Binding);
        BindType(Bit);
        BindType(Color);
        BindType(Int);
        BindType(Lib);
        BindType(Macro);
        BindType(Meta);
        BindType(Method);
        BindType(String);
        BindType(UserMethod);

        Bind("F", Value.F);
        Bind("_", Value.Nil);
        Bind("T", Value.T);

        BindMacro("^", [], (loc, target, vm, args) =>
         {
            var name = "";
            var f = args.Pop();

            if (f is Forms.Id id) {
                    name = id.Name;
                    f = args.Pop();
            }

            (string, int)[] fas;
 
            var skip = new Label();
            vm.Emit(Ops.Goto.Make(skip));
            var parentEnv = vm.Env;
            vm.PushEnv();

            if (f is Forms.Array af) {
                fas = af.Items.Select(f => {
                    if (f is Forms.Id id) {
                        var r = vm.AllocRegister();
                        vm.Env.Bind(id.Name, Value.Make(Core.Binding, new Binding(0, r)));
                        return (id.Name, r);
                    }

                    throw new EmitError(f.Loc, $"Invalid method arg: {f}");
                }).ToArray();
            } else {
                throw new EmitError(loc, "Invalid method args");
            }

            var m = new UserMethod(loc, vm.EmitPC, name, fas);
            var v = Value.Make(Core.UserMethod, m);

            if (name != "") {
                parentEnv.Bind(name, v);
            }

            vm.Emit(Ops.EnterMethod.Make(m));
            args.Emit(vm);
            vm.Emit(Ops.ExitMethod.Make());
            skip.PC = vm.EmitPC;
            vm.PopEnv();

            if (name == "") {
                vm.Emit(Ops.Push.Make(v));
            }
         });

        BindMethod("=", ["x", "y"], (loc, target, vm, stack, arity) =>
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

            stack.Push(Value.Make(Core.Bit, res));
        });

        BindMethod("+", [], (loc, target, vm, stack, arity) =>
        {
            var res = 0;

            while (arity > 0)
            {
                res += stack.Pop().Cast(loc, Core.Int);
                arity--;
            }

            stack.Push(Core.Int, res);
        });

        BindMethod("-", [], (loc, target, vm, stack, arity) =>
        {
            var res = 0;

            if (arity > 0)
            {
                if (arity == 1)
                {
                    res = -stack.Pop().Cast(loc, Core.Int);

                }
                else
                {
                    stack.Reverse(arity);
                    res = stack.Pop().Cast(loc, Core.Int);
                    arity--;

                    while (arity > 0)
                    {
                        res -= stack.Pop().Cast(loc, Core.Int);
                        arity--;
                    }
                }
            }

            stack.Push(Core.Int, res);
        });

        BindMethod("*", ["x", "y"], (loc, target, vm, stack, arity) =>
         {
             var res = stack.Pop().Cast(loc, Core.Int);
             arity--;

             while (arity > 0)
             {
                 res *= stack.Pop().Cast(loc, Core.Int);
                 arity--;
             }

             stack.Push(Core.Int, res);
         });

        BindMethod("/", ["x", "y"], (loc, target, vm, stack, arity) =>
        {
            stack.Reverse(arity);
            var res = stack.Pop().Cast(loc, Core.Int);
            arity--;

            while (arity > 0)
            {
                res /= stack.Pop().Cast(loc, Core.Int);
                arity--;
            }

            stack.Push(Core.Int, res);
        });

        BindMacro("check", ["expected", "body"], (loc, target, vm, args) =>
         {
             var ef = args.Pop();

             if (ef is null)
             {
                 throw new EmitError(loc, "Missing expected value");
             }

             vm.Emit(Ops.BeginFrame.Make());
             vm.PushEnv();

             try
             {
                 while (true)
                 {
                     if (args.Pop() is Form bf)
                     {
                         bf.Emit(vm, args);
                     }
                     else
                     {
                         break;
                     }
                 }

                 vm.Emit(Ops.EndFrame.Make());
                 ef.Emit(vm, args);
                 vm.Emit(Ops.Check.Make(loc, ef));
             }
             finally
             {
                 vm.PopEnv();
             }
         });

        BindMacro("decode", [], (loc, target, vm, args) =>
        {
            var skip = new Label();
            vm.Emit(Ops.Goto.Make(skip));
            var startPC = vm.EmitPC;
            args.Emit(vm);
            skip.PC = vm.EmitPC;
            vm.Decode(startPC);
        });

        BindMacro("define", ["id", "value"], (loc, target, vm, args) =>
        {
            while (true)
            {
                var id = args.Pop();

                if (id is null)
                {
                    break;
                }

                if (args.Pop() is Form f && vm.Eval(f, args) is Value v)
                {
                    vm.Env.Bind(((Forms.Id)id).Name, v);
                }
                else
                {
                    throw new EmitError(loc, "Missing value");
                }
            }
        });

        BindMacro("do", [], (loc, target, vm, args) =>
        {
            vm.Emit(Ops.BeginFrame.Make());
            vm.PushEnv();

            try
            {
                args.Emit(vm);
            }
            finally
            {
                vm.PopEnv();
            }

            vm.Emit(Ops.EndFrame.Make());
        });

        BindMacro("let", ["bindings"], (loc, target, vm, args) =>
        {
            if (args.Pop() is Forms.Array bsf)
            {
                var bs = bsf.Items;
                vm.Emit(Ops.BeginFrame.Make());
                vm.PushEnv();

                try
                {
                    var valueArgs = new Form.Queue();

                    for (var i = 0; i < bs.Length; i++)
                    {
                        var idf = bs[i];
                        i++;
                        var vf = bs[i];
                        var r = vm.AllocRegister();
                        vm.Env.Bind(((Forms.Id)idf).Name, Value.Make(Core.Binding, new Binding(0, r)));
                        vf.Emit(vm, valueArgs);
                        vm.Emit(Ops.SetRegister.Make(0, r));
                    }

                    while (true)
                    {
                        if (args.Pop() is Form f)
                        {
                            f.Emit(vm, args);
                        }
                        else
                        {
                            break;
                        }
                    }

                    vm.Emit(Ops.EndFrame.Make());
                }
                finally
                {
                    vm.PopEnv();
                }
            }
            else
            {
                throw new EmitError(loc, "Missing bindings");
            }
        });

        BindMacro("lib", [], (loc, target, vm, args) =>
        {
            if (args.Count == 0)
            {
                vm.Emit(Ops.Push.Make(Value.Make(Core.Lib, vm.Lib)));
            }
            else if (args.Pop() is Forms.Id nf)
            {
                var prevEnv = vm.Env;
                Lib? lib = null;

                if (vm.Env.Find(nf.Name) is Value v)
                {
                    lib = v.Cast(loc, Core.Lib);
                }
                else
                {
                    lib = new Lib(nf.Name, vm.Env);
                    vm.Env.BindLib(lib);
                }

                vm.Env = lib;

                if (!args.Empty)
                {
                    try
                    {
                        args.Emit(vm);
                    }
                    finally
                    {
                        vm.Env = prevEnv;
                    }
                }
            }
            else
            {
                throw new EmitError(loc, "Invalid library name");
            }
        });

        BindMacro("load", ["path"], (loc, target, vm, args) =>
        {
            while (true)
            {
                if (args.Pop() is Form pf)
                {
                    if (vm.Eval(pf, args) is Value p)
                    {
                        vm.Load(p.Cast(pf.Loc, Core.String));
                    }
                    else
                    {
                        throw new EvalError(pf.Loc, "Missing path");
                    }
                }
                else
                {
                    break;
                }
            }
        });

        BindMethod("rgb", ["r", "g", "b"], (loc, target, vm, stack, arity) =>
        {
            int b = stack.Pop().Cast(Core.Int);
            int g = stack.Pop().Cast(Core.Int);
            int r = stack.Pop().Cast(Core.Int);

            stack.Push(Core.Color, System.Drawing.Color.FromArgb(255, r, g, b));
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
    }
}