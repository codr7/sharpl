(^fib-rec [n]
  (if-else (< n 2) n (+ (fib-rec (- n 1)) (fib-rec (- n 2)))))

(say (fib-rec 10))