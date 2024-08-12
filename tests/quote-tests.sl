(check 'foo42
  (sym "foo" 42))

(check (= 'foo 'foo))

(check F
  (= 'foo 'bar))

(check ['foo 'bar 'baz]
  '[foo bar baz])

(check ['foo 42 'baz]
  (let [bar 42]
    '[foo ,bar baz]))

(check ['foo 42 "abc" 'qux]
  (let [bar 42 
      baz "abc"]
  '[foo ,[bar baz]* qux]))

(check '42
  ,''42)

(check '(foo 42) 
  (let [bar 42] 
    '(foo ,bar)))

(check F
  (= (Sym 'foo) (Sym 'foo)))