(load "fib.sl")

(say (benchmark 100 (fib-rec 20)))