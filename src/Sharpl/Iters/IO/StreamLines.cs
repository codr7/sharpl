namespace Sharpl.Iters.IO;

using Sharpl.Libs;

public class StreamLines : Sharpl.Iter
{
    public readonly TextReader Reader;

    public StreamLines(TextReader reader)
    {
        Reader = reader;
    }

    public override bool Next(VM vm, Register result, Loc loc)
    {
        var line = Reader.ReadLine();
        
        if (line is string s)
        {
            vm.Set(result, Value.Make(Core.String, line));
            return true;
        }
        
        return false;
    }
}