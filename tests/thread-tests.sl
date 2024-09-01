(check 42
  (let [p (spawn [p] (p 42))]
    (p)))