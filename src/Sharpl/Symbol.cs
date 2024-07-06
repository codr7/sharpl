using System.Reflection.Metadata.Ecma335;

namespace Sharpl;

public class Symbol {
    public readonly string Name;

    public Symbol(string name) {
        Name = name;
    }

    public override string ToString()
    {
        return $"'{Name}";
    }
}