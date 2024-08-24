namespace Sharpl;

public class Sym(string name)
{
    public string Name => name;
    public override string ToString() => $"'{Name}";
}