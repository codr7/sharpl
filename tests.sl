(check 42 
  42)

(check 42
  (do 42))

(check 42
  (define foo 42)
  foo)

(check 3
  (+ 1 2))
  
(check 2
  (- 3 1))

(check (- 2)
  (- 1 3))