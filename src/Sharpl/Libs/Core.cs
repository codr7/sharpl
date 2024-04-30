namespace Sharpl.Libs;

using System.Text;

public class Core : Lib
{
    public class IntType : Type<int>
    {
        public IntType(string name) : base(name) { }

        public override void Dump(Value value, StringBuilder result) {
            result.Append(value.ToString());
        }
    }

    public class LibType : Type<Lib>
    {
        public LibType(string name) : base(name) { }
    }

    public class MetaType : Type<AnyType>
    {
        public MetaType(string name) : base(name) { }
    }

    public readonly IntType Int = new IntType("Int");
    public readonly LibType Lib = new LibType("Lib");
    public readonly MetaType Meta = new MetaType("Meta");


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