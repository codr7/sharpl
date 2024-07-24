(check []
  [_*])
  
(check 6
  (reduce + [1 2 3] 0))

(check 6
  (reduce + (range 4) 0))
