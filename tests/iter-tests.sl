(check []
  [_*])
  
(check (List 'foo:1 'bar:2 'baz:3)
  (map Pair '[foo bar baz] [1 2 3 4]))

(check 6
  (reduce + [1 2 3] 0))

(check 6
  (reduce + (range _ 4) 0))

(check 5:2
  (find-first (^[x] (> x 3)) [1 3 5 7 9]))