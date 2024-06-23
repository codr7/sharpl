namespace Sharpl;

using Libs;

public class Lib: Env
{
    public Lib(string name, Env? parent, HashSet<string> ids): base(parent, ids)
    {
        Name = name;
    }

    public string Name { get; }

    public override string ToString() {
        return $"(Lib {Name})";
    }
}