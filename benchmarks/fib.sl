(load "../fib.sl")

(say (bench 100 (fib-rec 20)))
(say (bench 10000 (fib-tail 70 0 1)))
(say (bench 10000 (fib-list 70 (List 0))))
(say (bench 10000 (fib-map 70 {})))
