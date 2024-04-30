namespace Sharpl.Libs;

using Sharpl.Types.Core;

public class Core : Lib
{
    public static readonly IntType Int = new IntType("Int");
    public static readonly LibType Lib = new LibType("Lib");
    public static readonly MetaType Meta = new MetaType("Meta");
    public static readonly NilType Nil = new NilType("Nil");

    public Core() : base("core", null)
    {
        BindType(Int);
        BindType(Lib);
        BindType(Meta);
    }

    public void BindType(AnyType t)
    {
        Bind(t.Name, new Value(Meta, t));
    }
}