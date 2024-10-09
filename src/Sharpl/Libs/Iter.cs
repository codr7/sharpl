using Sharpl.Iters;
using Sharpl.Iters.Core;
using Sharpl.Types.Core;

namespace Sharpl.Libs;

public class Iter : Lib
{
    public Iter() : base("iter", null, [])
    {
        BindMethod("filter", ["pred", "seq"], (vm, target, arity, result, loc) =>
        {
            var pred = vm.GetRegister(0, 0);
            var seq = vm.GetRegister(0, 1);
            if (seq.Type is IterTrait it) 
                vm.Set(result, Value.Make(Core.Iter, new FilterItems(pred, it.CreateIter(seq, vm, loc))));
            else throw new EvalError("Not iterable", loc);
        });

        BindMacro("find-first", ["pred", "seq"], (vm, target, args, result, loc) =>
        {
            var pred = new Register(0, vm.AllocRegister());
            vm.Emit(args.Pop(), pred);

            var iter = new Register(0, vm.AllocRegister());
            vm.Emit(args.Pop(), iter);
            
            vm.Emit(Ops.CreateIter.Make(iter, loc));
            
            var index = new Register(0, vm.AllocRegister());
            vm.Emit(Ops.SetRegisterDirect.Make(index, Value.Make(Core.Int, 0)));
            
            var start = new Label(vm.EmitPC);
            var fail = new Label();
            var ok = new Label();
            vm.Emit(Ops.IterNext.Make(iter, new Register(0, 0), fail, loc));
            vm.Emit(Ops.CallRegister.Make(pred, 1, false, vm.NextRegisterIndex, vm.Result, loc));
            var next = new Label();
            vm.Emit(Ops.Branch.Make(vm.Result, next, loc));
            vm.Emit(Ops.CreatePair.Make(result, vm.Result, index, loc));
            vm.Emit(Ops.Goto.Make(ok));
            next.PC = vm.EmitPC;
            vm.Emit(Ops.Drop.Make(1));
            vm.Emit(Ops.Increment.Make(index, 1));
            vm.Emit(Ops.Goto.Make(start));
            fail.PC = vm.EmitPC;
            vm.Emit(Ops.Push.Make(Value._));
            ok.PC = vm.EmitPC;
        });

        BindMacro("for", ["vars", "body?"], (vm, target, args, result, loc) =>
        {
            vm.Emit(Ops.BeginFrame.Make(vm.NextRegisterIndex));

            vm.DoEnv(new Env(vm.Env, args.CollectIds()), loc, () =>
             {
                 var bindings = new List<(Register, Register)>();

                 if (args.Pop() is Forms.Array vfs)
                 {
                     for (var i = 0; i < vfs.Items.Length; i += 2)
                     {
                         var idForm = vfs.Items[i];
                         var valForm = vfs.Items[i + 1];
                         var seqReg = new Register(0, vm.AllocRegister());
                         vm.Emit(valForm, seqReg);
                         vm.Emit(Ops.CreateIter.Make(seqReg, loc));
                         var itReg = -1;

                         if (idForm is Forms.Id idf)
                         {
                             itReg = vm.AllocRegister();
                             vm.Env.Bind(idf.Name, Value.Make(Core.Binding, new Register(0, itReg)));
                         }
                         else if (idForm is not Forms.Nil) throw new EmitError("Expected id: " + idForm, loc);

                         bindings.Add((seqReg, new Register(0, itReg)));
                     }
                 }
                 else throw new EmitError("Invalid loop bindings", loc);

                 var end = new Label();
                 var start = new Label(vm.EmitPC);
                 foreach (var (seqReg, itReg) in bindings) vm.Emit(Ops.IterNext.Make(seqReg, itReg, end, loc));
                 args.Emit(vm, result);
                 vm.Emit(Ops.Goto.Make(start));
                 end.PC = vm.EmitPC;
                 vm.Emit(Ops.EndFrame.Make(loc));
             });
        });
        
        BindMethod("map", ["result", "seq1", "seq2?"], (vm, target, arity, result, loc) =>
        {
            var res = vm.GetRegister(0, 0);
            var sources = new Sharpl.Iter[arity];
            
            for (var i = 1; i < arity; i++)
            {
                var s = vm.GetRegister(0, i);
                if (s.Type is IterTrait it) { sources[i-1] = it.CreateIter(s, vm, loc);  }
                else { throw new EvalError($"Not iterable: {s.Dump(vm)}", loc); }
            }

            vm.Set(result, Value.Make(Core.Iter, new MapItems(res, sources)));
        });

        BindMacro("reduce", ["method", "sequence", "seed"], (vm, target, args, result, loc) =>
              {
                  var iter = new Register(0, vm.AllocRegister());
                  var methodForm = args.Pop();
                  var sequenceForm = args.Pop();
                  var seedForm = args.Pop();
                  var emptyArgs = new Form.Queue();

                  var method = new Register(0, vm.AllocRegister());
                  vm.Emit(methodForm, method);

                  vm.Emit(sequenceForm, iter);
                  vm.Emit(Ops.CreateIter.Make(iter, loc));

                  var acc = new Register(0, vm.AllocRegister());
                  vm.Emit(seedForm, acc);

                  var it = new Register(0, vm.AllocRegister());
                  var start = new Label(vm.EmitPC);
                  var done = new Label();
                  vm.Emit(Ops.IterNext.Make(iter, new Register(0, 0), done, loc));
                  vm.Emit(Ops.CopyRegister.Make(acc, new Register(0, 1)));
                  vm.Emit(Ops.CallRegister.Make(method, 2, false, vm.NextRegisterIndex, acc, loc));
                  vm.Emit(Ops.Goto.Make(start));
                  done.PC = vm.EmitPC;
                  vm.Emit(Ops.CopyRegister.Make(acc, result));
              });

        BindMethod("zip", ["in1", "in2", "in3?"], (vm, target, arity, result, loc) =>
        {
            Sharpl.Iter[] sources = new Sharpl.Iter[arity];

            for (var i = 0; i < arity; i++)
            {
                var s = vm.GetRegister(0, i);
                if (s.Type is IterTrait it) { sources[i] = it.CreateIter(s, vm, loc); }
                else { throw new EvalError($"Not iterable: {s}", loc); }
            }

            vm.Set(result, Value.Make(Core.Iter, new Zip(sources)));
        });
    }

    protected override void OnInit(VM vm)
    {
        Import(vm.CoreLib);

        vm.Eval("""
          (^enumerate [i in]
            (zip (range i) in))

          (^sum [in*]
            (reduce + in 0))
        """);
    }
}