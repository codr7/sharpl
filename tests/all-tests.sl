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

(check 3:4
  (let [foo 1 bar 2]
    (set foo 3 bar 4)
    foo:bar))

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

(check (= (emit '(+ 1 2)) (eval '(+ 1 2))))

(check 2
  (let [x 1]
    (inc x)))
  
(check 3
  (let [x 1]
    (inc x 2)))

(check 1
  (let [x 2]
    (dec x)))

(check 1
  (let [x 3]
    (dec x 2)))

(check 1
  (min 3 1 2))

(check 3
  (max 1 3 2))

(check 0
  (rand-int 1))
  
(check F 
  (is (rand-int) (rand-int)))

(check (is (type-of 42) Int))
(check (isa 42 Int))
(check (not (isa 'foo Int)))

(load "array-tests.sl")
(load "bind-tests.sl")
(load "char-tests.sl")
(load "defer-tests.sl")
(load "error-tests.sl")
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
(load "pipe-tests.sl")
(load "quote-tests.sl")
(load "string-tests.sl")
(load "time-tests.sl")
(load "thread-tests.sl")
(load "type-tests.sl")

(load "../examples/aoc23/code1-1.sl")
(load "../examples/aoc23/code2-1.sl")