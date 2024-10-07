(check "bummer"
  (try [Any:(^[e] e)]
    (fail _ 'bummer)))

(do
  (trait Foo)

  (data Bar [Pair Foo] 
    (^[x y z] x:y:z))

  (check (Bar 1 2 3)
    (try [Foo:(^[e] e)]
      (fail Bar 3 2 1))))

(check 'caught
  (try [_:(^[_] 'caught)] (+ 'foo)))
