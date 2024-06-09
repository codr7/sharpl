using Sharpl;
using Sharpl.Libs;
using Ops = Sharpl.Ops;

var vm = new VM();
vm.UserLib.Import(vm.CoreLib);
vm.UserLib.Import(vm.TermLib);

if (args.Length == 0) {
    vm.REPL();
} else {
    var startPC = vm.EmitPC;

    var vs = new Value[args.Length-1];

    for (var i = 0; i < vs.Length; i++) {
        vs[i] = Value.Make(Core.String, args[i+1]);
    }
    
    vm.UserLib.Bind("ARGS", Value.Make(Core.Array, vs));
    vm.Load(args[0], vm.UserLib);
    vm.Emit(Ops.Stop.Make());
    var stack = new Stack(VM.STACK_SIZE);
    vm.Eval(startPC, stack);
}