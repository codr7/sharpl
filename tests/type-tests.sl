﻿(do 
  (type Foo)
  (type Bar [Foo])
  (check (< Foo Bar))
  (check (not (< Bar Foo)))
  (check (> Bar Foo))
  (check (not (> Foo Bar))))