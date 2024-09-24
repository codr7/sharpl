using Sharpl;
using Sharpl.Libs;
using Ops = Sharpl.Ops;

var vm = new VM(VM.DEFAULT);
var mode = Mode.REPL;
var argOffset = 0;

if (args.Length > 0) {
    switch (args[0]) {
        case "dmit":
            mode = Mode.DMIT;
            argOffset += 2;
            break;
        case "repl":
            mode = Mode.REPL;
            argOffset++;
            break;
        case "run":
            mode = Mode.RUN;
            argOffset += 2;
            break;
        default:
            mode = Mode.RUN;
            argOffset = 1;
            break;
    }
};

var startPC = vm.EmitPC;
var vs = new Value[args.Length - argOffset];
for (var i = 0; i < vs.Length; i++) { vs[i] = Value.Make(Core.String, args[i + argOffset]); }
vm.UserLib.Bind("ARG", Value.Make(Core.Array, vs));

if (mode == Mode.REPL)
{
    new REPL().Run(vm);
}
else
{
    if (args.Length > argOffset - 1) { vm.Load(args[argOffset - 1]); }
    vm.Emit(Ops.Stop.Make());
    if (mode == Mode.RUN) { vm.Eval(startPC); }
    else if (mode == Mode.DMIT) { vm.Dmit(startPC); }
}
enum Mode { DMIT, RUN, REPL };