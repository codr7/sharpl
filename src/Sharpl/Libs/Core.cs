namespace Sharpl.Libs;

using Sharpl.Types.Core;

using System.Text;

public class Core : Lib
{
    public static readonly IntType Int = new IntType("Int");
    public static readonly LibType Lib = new LibType("Lib");
    public static readonly MetaType Meta = new MetaType("Meta");
    public static readonly MethodType Method = new MethodType("Method");
    public static readonly NilType Nil = new NilType("Nil");
    public static readonly PrimType Prim = new PrimType("Prim");
    public static readonly StringType String = new StringType("String");

    public Core() : base("core", null)
    {
        BindType(Int);
        BindType(Lib);
        BindType(Meta);
        BindType(Method);
        BindType(Prim);
        BindType(String);

        BindMethod("+", [], (loc, target, vm, stack, arity, recursive) =>
        {
            var res = 0;

            while (arity > 0)
            {
                res += stack.Pop().Cast(loc, Core.Int);
                arity--;
            }

            stack.Push(Core.Int, res);
        });

        BindMethod("-", [], (loc, target, vm, stack, arity, recursive) =>
        {
            var res = 0;

            if (arity > 0)
            {
                res = stack.Pop().Cast(loc, Core.Int);

                if (arity == 1)
                {
                    res = -res;
                }
                else
                {
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

        BindMethod("say", [], (loc, target, vm, stack, arity, recursive) =>
        {
            var res = new StringBuilder();

            while (arity > 0)
            {
                stack.Pop().Say(res);
                arity--;
            }

            Console.WriteLine(res.ToString());
        });

        BindMethod("string", [], (loc, target, vm, stack, arity, recursive) =>
        {
            var res = new StringBuilder();

            while (arity > 0)
            {
                stack.Pop().Say(res);
                arity--;
            }

            stack.Push(Core.String, res.ToString());
        });
    }


    public Method BindMethod(string name, string[] args, Method.BodyType body)
    {
        var m = new Method(name, args, body);
        Bind(m.Name, Value.Make(Method, m));
        return m;
    }

    public void BindType(AnyType t)
    {
        Bind(t.Name, Value.Make(Meta, t));
    }
}