namespace Sharpl.Libs;

using Sharpl.Types.Core;
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
    public static readonly PrimType Prim = new PrimType("Prim");
    public static readonly StringType String = new StringType("String");

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
        BindType(Prim);
        BindType(String);

        Bind("F", Value.F);
        Bind("_", Value.Nil);
        Bind("T", Value.T);

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

        BindMacro("check", ["expected", "body"], (loc, target, vm, env, args) =>
         {
             var ef = args.Pop();

             if (ef is null)
             {
                 throw new EmitError(loc, "Missing expected value");
             }

            var bodyEnv = new Env(env);

             while (true)
             {
                 if (args.Pop() is Form bf)
                 {
                     bf.Emit(vm, bodyEnv, args);
                 }
                 else
                 {
                     break;
                 }
             }

             ef.Emit(vm, new Env(env), args);
             vm.Emit(Ops.Check.Make(loc, ef));
         });

        BindMacro("define", ["id", "value"], (loc, target, vm, env, args) =>
        {
            while (true)
            {
                var id = args.Pop();

                if (id is null)
                {
                    break;
                }

                if (args.Pop() is Form f && vm.Eval(f, env, args) is Value v)
                {
                    env.Bind(((Forms.Id)id).Name, v);
                }
                else
                {
                    throw new EmitError(loc, "Missing value");
                }
            }
        });

        BindMacro("do", [], (loc, target, vm, env, args) =>
        {
            args.Emit(vm, new Env(env));
        });

        BindMacro("load", ["path"], (loc, target, vm, env, args) =>
        {
            while (true)
            {
                if (args.Pop() is Form pf)
                {
                    if (vm.Eval(pf, env, args) is Value p)
                    {
                        vm.Load(p.Cast(pf.Loc, Core.String), env);
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