namespace Sharpl;

using Libs;

public class Lib: Env
{
    public Lib(string name, Env? parent, string[] ids): base(parent, ids)
    {
        Name = name;
    }

    public string Name { get; }

    public override string ToString() {
        return $"(Lib {Name})";
    }
}