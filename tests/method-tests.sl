(check 2
  (^foo []
    2)

  (^bar []
    1
    (return (foo))
    3)

  (bar))
