(check {1:4 2:5 3:6}
  (let [result {}]
    (for [i [1 2 3] 
          j [4 5 6]]
      (result i j))
    result))