(^fib-rec [n]  
  (else (< n 2) n (+ (fib-rec (dec n)) (fib-rec (dec n)))))

(^fib-tail [n a b]
  (else (> n 1) (return (fib-tail (dec n) b (+ a b))) (else (is n 0) a b)))

(^ fib-list [n l]
  (else (< n (len l))
    (l n)
    (let [result (else (< n 2) n (+ (fib-list (dec n) l)
                                    (fib-list (dec n) l)))] 
      (push l result)
	    result)))

(^ fib-map [n m]
  (or (m n)
      (else (< n 2) 
        n 
        (let [result (+ (fib-map (- n 1) m)
                        (fib-map (- n 2) m))] 
          (m n result)
	        result))))
