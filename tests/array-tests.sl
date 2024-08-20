(check []
  [[[]*]*])

(check [42]
  [[[42]*]*])

(check 42
  (- [43 1]*))

(check [1 4 3]
  (let [foo [1 2 3]]
    (foo 1 4)
    foo))

(check [1 2 3 4 5]
  [1 2 [3 4]* 5])

(check F
  (is [1 2 3] [1 2 3]))

(check 2
  (let [x [1 2 3]]
    (x 1)))
    
(check [1 4 3]
  (let [x [1 2 3]]
    (x 1 4)
    x))

(check [2 3]
  ([1 2 3 4] 1:2))