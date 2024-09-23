namespace Sharpl.Iters.IO;

using Sharpl.Libs;

public class StreamLines : Sharpl.Iter
{
    public readonly TextReader Reader;

    public StreamLines(TextReader reader)
    {
        Reader = reader;
    }

    public override Value? Next(VM vm, Loc loc)
    {
        var line = Reader.ReadLine();
        return (line == null) ? null : Value.Make(Core.String, line);
    }
}