namespace Sharpl;

public abstract class Form {
    public static void Emit(EmitArgs args, VM vm, Lib lib) {
        while (args.Count > 0) {
            if (args.Pop() is Form v) {
                v.Emit(vm, lib, args);
            }
        }
    }
    
    public readonly Loc Loc;

    protected Form(Loc loc) {
        Loc = loc;
    }

    public abstract void Emit(VM vm, Lib lib, EmitArgs args);
    
    public virtual void EmitCall(VM vm, Lib lib, EmitArgs args) {
        Emit(args, vm, lib);
        Emit(vm, lib, new EmitArgs());
        vm.Emit(Ops.CallIndirect.Make(Loc, args.Count));
    }
}