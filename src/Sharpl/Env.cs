namespace Sharpl;

using Libs;

public class Env
{
    private Dictionary<string, Value> bindings = new Dictionary<string, Value>();

    public Env(Env? parent, string[] ids)
    {
        Parent = parent;

        for (var p = parent; p is Env; p = p.Parent) {
            foreach (var id in ids) {
                if (p.bindings.ContainsKey(id) && p.bindings[id] is Value b && b.Type == Core.Binding) {
                    var v = b.Cast(Core.Binding);
                    Bind(id, Value.Make(Core.Binding, new Binding(v.FrameOffset+1, v.Index)));
                }
            }
        }
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

    public void BindLib(Lib lib) {
            Bind(lib.Name, Value.Make(Core.Lib, lib));
    }
    
    public Macro BindMacro(string name, string[] args, Macro.BodyType body)
    {
        var m = new Macro(name, args, body);
        Bind(m.Name, Value.Make(Core.Macro, m));
        return m;
    }

    public Method BindMethod(string name, string[] args, Method.BodyType body)
    {
        var m = new Method(name, args, body);
        Bind(m.Name, Value.Make(Core.Method, m));
        return m;
    }

    public void BindType(AnyType t)
    {
        Bind(t.Name, Value.Make(Core.Meta, t));
    }

    public Value? Find(string id)
    {
        return bindings.ContainsKey(id) ? bindings[id] : Parent?.Find(id);
    }

    public void Import(Env source) {
        foreach (var (id, v) in source.bindings) {
            Bind(id, v);
        }
    }

    public Env? Parent { get; }

    public bool Unbind(string id)
    {
        return bindings.Remove(id);
    }
}