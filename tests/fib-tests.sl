(load "../fib.sl")

(check 55
  (fib-rec 10))

(check 55
  (fib-list 10 (List 0)))

(check 55
  (fib-map 10 {}))

(check 55
  (fib-tail 10 0 1))