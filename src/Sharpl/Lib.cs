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
        vm.Env.BindLib(this);

        vm.DoEnv(this, () =>
        {
            OnInit(vm);
        });
    }

    public string Name { get; }

    public override string ToString()
    {
        return $"(Lib {Name})";
    }

    protected virtual void OnInit(VM vm) { }
}