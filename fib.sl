(^fib-rec [n]
  (if-else (< n 2) n (+ (fib-rec (dec n)) (fib-rec (dec n)))))