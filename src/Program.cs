using Sharpl;

var vm = new VM();
vm.UserLib.Import(vm.CoreLib);
vm.UserLib.Import(vm.TermLib);
vm.REPL();