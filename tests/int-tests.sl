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

(check [1 2]
  [1..3*])