using Sharpl.Iters;
using Sharpl.Iters.Core;
using Sharpl.Types.Core;

namespace Sharpl.Libs;

public class Iter : Lib
{
    public Iter() : base("iter", null, [])
    {
        BindMethod("filter", ["pred", "seq"], (vm, stack, target, arity, loc) =>
        {
            var seq = stack.Pop();
            var pred = stack.Pop();
            if (seq.Type is IterTrait it) { stack.Push(Core.Iter, new FilterItems(pred, it.CreateIter(seq, vm, loc))); }
            else { throw new EvalError("Not iterable", loc); }
        });

        BindMacro("find-first", ["pred", "seq"], (vm, target, args, loc) =>
        {
            var pred = new Register(0, vm.AllocRegister());
            vm.Emit(args.Pop());
            vm.Emit(Ops.SetRegister.Make(pred));

            var iter = new Register(0, vm.AllocRegister());
            vm.Emit(args.Pop());
            vm.Emit(Ops.CreateIter.Make(loc, iter));

            var index = new Register(0, vm.AllocRegister());
            vm.Emit(Ops.Push.Make(Value.Make(Core.Int, 0)));
            vm.Emit(Ops.SetRegister.Make(index));

            var start = new Label(vm.EmitPC);
            var fail = new Label();
            var ok = new Label();
            vm.Emit(Ops.IterNext.Make(loc, iter, fail));
            vm.Emit(Ops.Repush.Make(1));
            vm.Emit(Ops.CallRegister.Make(loc, pred, 1, false, vm.NextRegisterIndex));
            var next = new Label();
            vm.Emit(Ops.Branch.Make(loc, next));
            vm.Emit(Ops.GetRegister.Make(index));
            vm.Emit(Ops.CreatePair.Make(loc));
            vm.Emit(Ops.Goto.Make(ok));
            next.PC = vm.EmitPC;
            vm.Emit(Ops.Drop.Make(1));
            vm.Emit(Ops.Increment.Make(index, 1));
            vm.Emit(Ops.Goto.Make(start));
            fail.PC = vm.EmitPC;
            vm.Emit(Ops.Push.Make(Value._));
            ok.PC = vm.EmitPC;
        });

        BindMacro("for", ["vars", "body?"], (vm, target, args, loc) =>
        {
            vm.Emit(Ops.BeginFrame.Make(vm.NextRegisterIndex));

            vm.DoEnv(new Env(vm.Env, args.CollectIds()), () =>
             {
                 var bindings = new List<(Register, Register)>();

                 if (args.Pop() is Forms.Array vfs)
                 {
                     for (var i = 0; i < vfs.Items.Length; i += 2)
                     {
                         var idForm = vfs.Items[i];
                         var valForm = vfs.Items[i + 1];
                         var seqReg = vm.AllocRegister();
                         vm.Emit(valForm);
                         vm.Emit(Ops.CreateIter.Make(loc, new Register(0, seqReg)));
                         var itReg = -1;

                         if (idForm is Forms.Id idf)
                         {
                             itReg = vm.AllocRegister();
                             vm.Env.Bind(idf.Name, Value.Make(Core.Binding, new Register(0, itReg)));
                         }
                         else if (idForm is not Forms.Nil) { throw new EmitError("Expected id: " + idForm, loc); }

                         bindings.Add((new Register(0, seqReg), new Register(0, itReg)));
                     }
                 }
                 else { throw new EmitError("Invalid loop bindings", loc); }

                 var end = new Label();
                 var start = new Label(vm.EmitPC);

                 foreach (var (seqReg, itReg) in bindings)
                 {
                     vm.Emit(Ops.IterNext.Make(loc, seqReg, end, push: itReg.Index != -1));
                     if (itReg.Index != -1) { vm.Emit(Ops.SetRegister.Make(itReg)); }
                 }

                 args.Emit(vm);
                 vm.Emit(Ops.Goto.Make(start));
                 end.PC = vm.EmitPC;
                 vm.Emit(Ops.EndFrame.Make());
             });
        });

        BindMacro("map", ["method", "sequence1"], (vm, target, args, loc) =>
                      {
                          var result = new Register(0, vm.AllocRegister());
                          vm.Emit(Ops.CreateList.Make(result));
                          var methodForm = args.Pop();
                          var iters = args.Select(a => (a, new Register(0, vm.AllocRegister()))).ToArray();

                          foreach (var (a, it) in iters)
                          {
                              vm.Emit(a);
                              vm.Emit(Ops.CreateIter.Make(loc, it));
                          }
                          var start = new Label(vm.EmitPC);
                          var end = new Label();
                          foreach (var (a, it) in iters) { vm.Emit(Ops.IterNext.Make(loc, it, end)); }
                          vm.Emit(methodForm);
                          vm.Emit(Ops.CallStack.Make(loc, args.Count, args.IsSplat, vm.NextRegisterIndex));
                          vm.Emit(Ops.PushItem.Make(loc, result));
                          vm.Emit(Ops.Goto.Make(start));
                          end.PC = vm.EmitPC;
                          vm.Emit(Ops.GetRegister.Make(result));
                          args.Clear();
                      });

        BindMacro("reduce", ["method", "sequence", "seed"], (vm, target, args, loc) =>
              {
                  var iter = new Register(0, vm.AllocRegister());
                  var methodForm = args.Pop();
                  var sequenceForm = args.Pop();
                  var seedForm = args.Pop();
                  var emptyArgs = new Form.Queue();

                  var method = new Register(0, vm.AllocRegister());
                  vm.Emit(methodForm);
                  vm.Emit(Ops.SetRegister.Make(method));

                  vm.Emit(sequenceForm);
                  vm.Emit(Ops.CreateIter.Make(loc, iter));
                  vm.Emit(seedForm);

                  var start = new Label(vm.EmitPC);
                  var done = new Label();
                  vm.Emit(Ops.IterNext.Make(loc, iter, done));
                  vm.Emit(Ops.CallRegister.Make(loc, method, 2, false, vm.NextRegisterIndex));
                  vm.Emit(Ops.Goto.Make(start));
                  done.PC = vm.EmitPC;
              });

        BindMethod("zip", ["in1", "in2", "in3?"], (vm, stack, target, arity, loc) =>
        {
            Sharpl.Iter[] sources = new Sharpl.Iter[arity];

            for (int i = arity - 1; i >= 0; i--)
            {
                var s = stack.Pop();
                if (s.Type is IterTrait it) { sources[i] = it.CreateIter(s, vm, loc); }
                else { throw new EvalError($"Not iterable: {s}", loc); }
            }

            stack.Push(Core.Iter, new Zip(sources));
        });
    }

    protected override void OnInit(VM vm)
    {
        Import(vm.CoreLib);

        vm.Eval("""
          (^enumerate [i in]
            (zip (range i) in))

          (^sum [in]
            (reduce + in 0))
        """);
    }
}