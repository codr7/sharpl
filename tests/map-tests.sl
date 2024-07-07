(check {'bar:2 'baz:3 'foo:1}
  {'foo:1 'bar:2 'baz:3})

(check {'bar:2 'baz:3 'foo:1}
  (Map ['foo:1 'bar:2 'baz:3]*))