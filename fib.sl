(^fib-rec [n]  
  (else (< n 2) n (+ (fib-rec (dec n)) (fib-rec (dec n)))))

(^fib-tail [n a b]
  (else (> n 1) (return (fib-tail (dec n) b (+ a b))) (else (is n 0) a b)))

(^ fib-list [n cache]
  (else (< n (len cache))
    (cache n)
    (let [result (else (< n 2) n (+ (fib-list (dec n) cache)
                                    (fib-list (dec n) cache)))] 
      (push cache result)
	    result)))

(^ fib-map [n cache]
  (or (cache n)
      (else (< n 2) 
        n 
        (let [result (+ (fib-map (- n 1) cache)
                        (fib-map (- n 2) cache))] 
          (cache n result)
	        result))))
