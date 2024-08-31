using Sharpl;
using Sharpl.Libs;
using Ops = Sharpl.Ops;

var vm = new VM(VM.DEFAULT);

if (args.Length == 0) {
    REPL.Run(vm);
} else {
    var startPC = vm.EmitPC;

    var vs = new Value[args.Length-1];

    for (var i = 0; i < vs.Length; i++) {
        vs[i] = Value.Make(Core.String, args[i+1]);
    }
    
    vm.UserLib.Bind("ARG", Value.Make(Core.Array, vs));
    vm.Load(args[0]);
    vm.Emit(Ops.Stop.Make());
    vm.Eval(startPC);
}