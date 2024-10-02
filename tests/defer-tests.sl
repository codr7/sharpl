(check 3
  (var foo 1)

  (do
    (defer (^[] (set foo 3)))
    (set foo 2))
  
  foo)