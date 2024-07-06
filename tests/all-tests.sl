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

(check 42
  (^foo [x]
    x)

  (foo 42))

(check 42
  (^foo [a b c]
    (- a b c))

  (foo 45 2 1))

(check 6
  (^foo [in out]
    (if-else (= in 0) out (foo (- in 1) (+ out in))))

  (foo 3 0))
    
(check 42
  (let [foo (let [bar 42]
              (^[] bar))]
    (foo)))
    
(check 42
  (let [f (^[x] x)]
    (f 42)))

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

(check T
  (= 42 42))

(check T
  (< 1 2 3))

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

(check "FOO"
  (string/up "Foo"))

(check "foo"
  (string/down "Foo"))

(check 42
  (- [43 1]*))

(check 6
  (reduce + [1 2 3] 0))

(load "array-tests.sl")
(load "fib-tests.sl")
(load "io-tests.sl")  
(load "lib-tests.sl")
(load "method-tests.sl")
(load "pair-tests.sl")
(load "rx-tests.sl")