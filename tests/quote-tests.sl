(check 'foo42
  (Sym "foo" 42))

(check (= 'foo 'foo))

(check F
  (= 'foo 'bar))

(check ['foo 'bar 'baz]
  '[foo bar baz])
