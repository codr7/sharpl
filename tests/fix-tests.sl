(check 1.23
  (Fix 2 123))

(check 1.5
  (+ 1.25 .25))

(check (- 1.23)
  (Fix 2 (- 123)))

(check 1.25
  (- 1.5 .25))

(check [1.0 1.5 2.0]
  [(range 1.0 2.5 .5)*])