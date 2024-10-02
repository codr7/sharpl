(check 3
  (var foo 1)

  (do
    (defer (^[] (set foo 3)))
    (set foo 2))
  
  foo)

  (check 2
    (var foo 1)

    (do
      (defer (^[] 
        (check (is foo 3))
        (set foo 2)))

      (defer (^[] 
        (check (is foo 4))
        (set foo 3)))
      (set foo 4))
  
    foo)