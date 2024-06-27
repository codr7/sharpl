namespace Sharpl.Types.IO;

using System.Text;
using Sharpl.Types.Core;
using Sharpl.Iters.IO;

public class ReadStreamType : Type<TextReader>, IterTrait
{
    public ReadStreamType(string name) : base(name) { }

    public override void Dump(Value value, StringBuilder result)
    {
        result.Append($"(ReadStream {value.Cast(this)})");
    }

    public Iter Iter(Value target)
    {
        return new StreamLines(target.Cast(this));
    }
}