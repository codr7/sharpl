namespace Sharpl.Libs;

public class Json : Lib
{
    public Json() : base("json", null, [])
    {
        BindMethod("decode", ["value"], (vm, target, arity, loc) =>
        {
            var jsLoc = new Loc("json");
            var v = Sharpl.Json.ReadValue(vm, new StringReader(stack.Pop().Cast(Core.String)), ref jsLoc);
            stack.Push((v is null) ? Value._ : (Value)v);
        });

        BindMethod("encode", ["value"], (vm, target, arity, result, loc) =>
            vm.Set(result, Value.Make(Core.String, vm.GetRegister(0, 0).ToJson(loc))));
    }
}