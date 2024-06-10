namespace Sharpl;

using Libs;

public class Lib: Env
{
    public Lib(string name, Env? parent): base(parent)
    {
        Name = name;
    }

    public string Name { get; }

    public override string ToString() {
        return $"(Lib {Name})";
    }
}