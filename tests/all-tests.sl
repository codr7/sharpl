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

(check 35
  (var foo 35)
  
  (^bar []
    foo)

  (let [foo (+ foo 7)]
    (check 42
      (bar)))
      
  (bar))

(check 42:2
  (parse-int "42foo"))

(check T
  (= (emit '(+ 1 2)) (eval '(+ 1 2))))

(check 1
  (min 3 1 2))

(check 3
  (max 1 3 2))

(load "array-tests.sl")
(load "char-tests.sl")
(load "fib-tests.sl")
(load "fix-tests.sl")
(load "int-tests.sl")
(load "io-tests.sl")  
(load "iter-tests.sl")
(load "json-tests.sl")
(load "lib-tests.sl")
(load "list-tests.sl")
(load "logic-tests.sl")
(load "loop-tests.sl")
(load "map-tests.sl")
(load "method-tests.sl")
(load "pair-tests.sl")
(load "quote-tests.sl")
(load "string-tests.sl")

(load "../aoc23/code1-1.sl")