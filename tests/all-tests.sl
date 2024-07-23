(check 42 
  42)

(check 42
  (do 42))

(check 42
  (let [x 42] 
    x))

(check 42
  (let [foo 35]
    (let [bar (+ foo 7)] 
      bar)))

(check 42
  (let [foo 35])
  (define foo 42)
  foo)

(check 42
  (define foo 42)
  foo)

(check 42
  (define foo (eval (+ 35 7)))
  foo)

(check 35
  (define foo 35)
  
  (^bar []
    foo)

  (let [foo (+ foo 7)]
    (check 42
      (bar)))
      
  (bar))

(check 1
  1 (if 0 2))

(check 2
  1 (if 42 2))

(check 2
  (if-else 0 1 2))

(check 1
  (if-else 42 1 2))

(check (= 42 42))

(check (< 1 2 3))

(check F
  (< 1 3 2))

(check 42
  (let [foo 43]
    (dec foo)))

(check 3
  (+ 1 2))
  
(check 2
  (- 3 1))

(check (- 2)
  (- 1 3))

(check 42
  (* 7 6))

(check 7
  (/ 42 6))

(check 2
  (let [x [1 2 3]]
    (x 1)))
    
(check [1 4 3]
  (let [x [1 2 3]]
    (x 1 4)
    x))

(check 42
  (or F 42))

(check 0
  (or F 0))

(check (or T 42))

(check 42
  (- [43 1]*))

(load "array-tests.sl")
(load "fib-tests.sl")
(load "io-tests.sl")  
(load "iter-tests.sl")
(load "lib-tests.sl")
(load "map-tests.sl")
(load "method-tests.sl")
(load "pair-tests.sl")
(load "quote-tests.sl")
(load "rx-tests.sl")
(load "string-tests.sl")