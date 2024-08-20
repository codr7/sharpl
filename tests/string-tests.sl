(check (is "abc" "abc"))

(check "FOO"
  (string/up "Foo"))

(check "foo"
  (string/down "Foo"))

(check 3
  (len "foo"))

(check "cba"
  (string/reverse "abc"))

(check "bar"
  ("foobarbaz" 2:3))

(check "bar"
  ("foobarbaz" 3:3))