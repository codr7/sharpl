namespace Sharpl;

using Libs;

public class Lib : Env
{
    public Lib(string name, Env? parent, HashSet<string> ids) : base(parent, ids)
    {
        Name = name;
    }

    public void Init(VM vm)
    {
        var prevEnv = vm.Env;
        prevEnv.BindLib(this);
        vm.Env = this;

        try
        {
            OnInit(vm);
        }
        finally
        {
            vm.Env = prevEnv;
        }
    }

    public string Name { get; }

    public override string ToString()
    {
        return $"(Lib {Name})";
    }

    protected virtual void OnInit(VM vm) { }
}