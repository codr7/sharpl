(load "../fib.sl")

(say (bench 100 (fib-rec 20)))
(say (bench 10000 (fib-tail 45 0 1)))
(say (bench 10000 (fib-list 45 (List 0))))
(say (bench 10000 (fib-map 45 {})))
