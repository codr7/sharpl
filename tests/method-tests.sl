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
  (let [f (^[x] x)]
    (f 42)))

(check 2
  (^foo []
    2)

  (^bar []
    1 (return (foo)) 3)

  (bar))

(check 42
  (let [foo (let [bar 44]
              (^[] (dec bar)))]
    (foo)
    (foo)))

(check 3
  (^foo [x]
    (+ x 1))
  
  (^bar [x]
    (+ x 2))

  ((comp foo bar) 0))

(check 35
  (^foo [bar*]
    (- bar*))
  
  (foo 42 7))
