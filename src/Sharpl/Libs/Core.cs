namespace Sharpl.Libs;

using Sharpl.Types.Core;

public class Core : Lib
{
    public static readonly BitType Bit = new BitType("Bit");
    public static readonly ColorType Color = new ColorType("Color");
    public static readonly IntType Int = new IntType("Int");
    public static readonly LibType Lib = new LibType("Lib");
    public static readonly MetaType Meta = new MetaType("Meta");
    public static readonly MethodType Method = new MethodType("Method");
    public static readonly NilType Nil = new NilType("Nil");
    public static readonly PrimType Prim = new PrimType("Prim");
    public static readonly StringType String = new StringType("String");

    public Core() : base("core", null)
    {
        BindType(Color);
        BindType(Bit);
        BindType(Int);
        BindType(Lib);
        BindType(Meta);
        BindType(Method);
        BindType(Prim);
        BindType(String);

        Bind("F", Value.F);
        Bind("_", Value.Nil);
        Bind("T", Value.T);

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

        BindMethod("rgba", ["r", "g", "b", "a"], (loc, target, vm, stack, arity, recursive) =>
        {
            int a = stack.Pop().Cast(Core.Int);
            int b = stack.Pop().Cast(Core.Int);
            int g = stack.Pop().Cast(Core.Int);
            int r = stack.Pop().Cast(Core.Int);


            stack.Push(Core.Color, System.Drawing.Color.FromArgb(a, r, g, b));
        });   

        BindMethod("rgb", ["r", "g", "b"], (loc, target, vm, stack, arity, recursive) =>
        {
            int b = stack.Pop().Cast(Core.Int);
            int g = stack.Pop().Cast(Core.Int);
            int r = stack.Pop().Cast(Core.Int);

            stack.Push(Core.Color, System.Drawing.Color.FromArgb(255, r, g, b));
        });     
    }
}