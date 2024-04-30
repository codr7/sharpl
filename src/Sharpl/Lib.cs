namespace Sharpl;

public class Lib
{
    private Dictionary<string, Value> bindings = new Dictionary<string, Value>();

    public Lib(string name, Lib? parentLib)
    {
        Name = name;
        ParentLib = parentLib;
    }

    public Value? this[string id]
    {
        get => Find(id);
        set
        {
            if (value == null)
            {
                Unbind(id);
            }
            else
            {
                Bind(id, (Value)value);
            }
        }
    }

    public void Bind(string id, Value value)
    {
        bindings[id] = value;
    }

    public Value? Find(string id)
    {
        return bindings.ContainsKey(id) ? bindings[id] : ParentLib?.Find(id);
    }

    public string Name { get; }
    public Lib? ParentLib { get; }

    public bool Unbind(string id)
    {
        return bindings.Remove(id);
    }
}