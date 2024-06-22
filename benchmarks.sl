(load "fib.sl")

(say (benchmark 100 (fib-rec 20)))
(say (benchmark 10000 (fib-tail 70 0 1)))