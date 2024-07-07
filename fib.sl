(^fib-rec [n]
  (if-else (< n 2) n (+ (fib-rec (dec n)) (fib-rec (dec n)))))

(^fib-tail [n a b]
  (if-else (> n 1) (return (fib-tail (dec n) b (+ a b))) (if-else (= n 0) a b)))

(^ fib-map [n m]
  (or (m n)
      (let [result (if (< n 2) n (+ (fib-map (- n 1) m)
                                    (fib-map (- n 2) m)))] 
        (m n result)
	result)))
