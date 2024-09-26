(check 2:1
  (var r:l 1:2)
  l:r)

(check 2
  (var _:r 1:2)
  r)

(check 2:1
  (let [r:l 1:2] l:r))

(check 2
  (let [_:r 1:2] r))