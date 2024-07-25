using System.Reflection.Metadata.Ecma335;

namespace Sharpl;

public class Sym {
    public readonly string Name;

    public Sym(string name) {
        Name = name;
    }

    public override string ToString()
    {
        return $"'{Name}";
    }
}