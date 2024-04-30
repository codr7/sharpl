using System.Text;

namespace Sharpl;

public readonly record struct Value(AnyType Type, dynamic Data)
{
    public static Value Make<T>(Type<T> type, dynamic data)
    {
        return new Value(type, data);
    }

    public static readonly Value Nil = Value.Make(Libs.Core.Nil, false);

    public void Dump(StringBuilder result) {
        Type.Dump(this, result);
    }

    public void Say(StringBuilder result) {
        Type.Say(this, result);
    }

    public override string ToString() {
        var res = new StringBuilder();
        Dump(res);
        return res.ToString();
    }
}