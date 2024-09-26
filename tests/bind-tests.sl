(check 2:1
  (var r:l 1:2)
  l:r)

(check 2:3
  (var _:r:rr 1:2:3)
  r:rr)

(check 2:1
  (let [r:l 1:2] l:r))

(check 2:3
  (let [_:r:rr 1:2:3] r:rr))