using System.Text;

namespace Sharpl;

public class AnyType
{
    public string Name { get; }

    public virtual void Dump(Value value, StringBuilder result) {
        result.Append(value.ToString());
    }

    public virtual void Say(Value value, StringBuilder result) {
        Dump(value, result);
    }

    public override string ToString() {
        return $"(Type {Name})";
    }

    protected AnyType(string name)
    {
        Name = name;
    }
}

public class Type<T> : AnyType
{
    public Type(string name) : base(name) { }
}