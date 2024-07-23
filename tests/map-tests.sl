(check {'bar:2 'baz:3 'foo:1}
  {'foo:1 'bar:2 'baz:3})

(check {'bar:2 'baz:3 'foo:1}
  (Map ['foo:1 'bar:2 'baz:3]*))

(check [1:2 3:4]
  [{3:4 1:2}*])

(check {1:2 3:4}
  {[3:4 1:2]*})

(let [m1 '{foo:1 bar:2} m2 (Map m1*)]
  (check (not (is m1 m2)))
  (check (= m1 m2)))