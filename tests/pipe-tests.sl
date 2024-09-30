(check 42
  (let [p1 (Pipe) p2 (Pipe)]
    (p2 42)
    ((poll [p1 p2]))))