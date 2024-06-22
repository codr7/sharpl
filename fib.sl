(^fib-rec [n]
  (if-else (< n 2) n (+ (fib-rec (dec n)) (fib-rec (dec n)))))

(^fib-tail [n a b]
  (if-else (> n 1) (fib-tail (dec n) b (+ a b)) (if-else (= n 0) a b)))
