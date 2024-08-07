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
  (var foo 42)
  foo)

(check 42
  (var foo 42)
  foo)

(check 42
  (var foo (eval (+ 35 7)))
  foo)

(check 35
  (var foo 35)
  
  (^bar []
    foo)

  (let [foo (+ foo 7)]
    (check 42
      (bar)))
      
  (bar))

(load "array-tests.sl")
(load "fib-tests.sl")
(load "int-tests.sl")
(load "io-tests.sl")  
(load "iter-tests.sl")
(load "lib-tests.sl")
(load "logic-tests.sl")
(load "map-tests.sl")
(load "method-tests.sl")
(load "pair-tests.sl")
(load "quote-tests.sl")
(load "rx-tests.sl")
(load "string-tests.sl")