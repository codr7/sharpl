(check (is "abc" "abc"))

(check "FOO"
  (string/up "Foo"))

(check "foo"
  (string/down "Foo"))
