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

(check "abc"
  (let [foo "ab"] 
    (push foo \c)
    foo))

(check \c
  (peek "abc")) 

(check \c:"ab"
  (let [foo "abc"] 
    (pop foo):foo))