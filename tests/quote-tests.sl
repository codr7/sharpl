(check 'foo42
  (Symbol "foo" 42))

(check T
  (= 'foo 'foo))

(check F
  (= 'foo 'bar))

(check ['foo 'bar 'baz]
  '[foo bar baz])
