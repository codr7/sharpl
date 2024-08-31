(check (List 1 2 3)
  (let [foo (List 1 2)] 
    (push foo 3)
    foo))

(check 3
  (peek (List 1 2 3)))

(check 3:(List 1 2)
  (let [foo (List 1 2 3)] 
    (pop foo):foo))

(check (List 2 3)
  ((List 1 2 3 4) 1:2))

(check (List 2 3)
  ((List 1 2 3) 1:_))

(check 3
  #(List 1 2 3))