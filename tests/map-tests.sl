(check {'bar:2 'baz:3 'foo:1}
  {'foo:1 'bar:2 'baz:3})

(check {'bar:2 'baz:3 'foo:1}
  (Map ['foo:1 'bar:2 'baz:3]*))

(check [1:2 3:4]
  [{3:4 1:2}*])

(check {1:2 3:4}
  {[3:4 1:2]*})