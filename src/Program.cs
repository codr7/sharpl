using System.Diagnostics;
using Sharpl;
using Sharpl.Operations;

var vm = new VM();
var Int = new Type<int>("Int");
var v = Value.Make(Int, 42);
vm.Emit(Push.Make(v));
vm.Emit(Stop.Make());
var stack = new Stack<Value>(32);
vm.Eval(0, stack);
Debug.Assert(stack.Peek() == v);