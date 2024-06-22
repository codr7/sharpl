(^fib-rec [n]
  (if (< n 2) n (+ (fib-rec (dec n)) (fib-rec (dec n)))))

(say (fib-rec 10))