(do 
  (trait Foo)
  (trait Bar Foo)

(check (< Foo Bar))
(check (not (< Bar Foo)))

(check (> Bar Foo))
(check (not (> Foo Bar))))

(check 5
  (data Foo Int)
  (+ (Foo 2) 3))

(check 2
  (data Bar Map
    (^[x y z] {x:1 y:2 z:3}))

  ((Bar 'a 'b 'c) 'b))