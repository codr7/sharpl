(check []
  [_*])
  
(check 6
  (reduce + [1 2 3] 0))

(check 6
  (reduce + (range _ 4) 0))
