(check F
  (Bit T:F))

(check F
  (Bit F:T))

(check (Bit T:T))

(check 1:4:3
  (let [foo 1:2:3]
    (foo 1 4)))

(check [1 2 3]
  [1:2:3*])

(check 1:2:3
  (Pair [1 2 3]*))