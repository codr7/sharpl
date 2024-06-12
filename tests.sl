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
  (^foo [x]
    x)

  (foo 42))

(check 42
  (let [f (^[x] x)]
    (f 42)))

(check 42
  (define foo 35)
  
  (^bar []
    foo)

  (let [foo (+ foo 7)]
    (bar)))