(check {1:4 2:5 3:6}
  (let [result {}]
    (for [i [1 2 3] 
          j [4 5 6]]
      (result i j))
    result))

(check (List 0 1 2)
  (^enumerate [n]
    (let [result (List) i 0]
      (loop
        (push result i)
        (if (is (inc i) n) (return result)))))
  
  (enumerate 3))