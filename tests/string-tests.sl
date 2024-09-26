(check (is "abc" "abc"))

(check "FOO"
  (string/up "Foo"))

(check "foo"
  (string/down "Foo"))

(check 3
  #"foo")

(check "cba"
  (string/reverse "abc"))

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

(check "AbAAbbA"
  (string/replace "abaabba" "a" "A"))

(check "foo-bar"
  (string/replace "foo  bar" "(\w+)\s*(\w+)" "$1-$2"))

(check ["foo" "bar"]
  (string/split "foo bar" " "))

(check "foobar"
  (string/strip ";foo;bar;" \;))

(check "foobar"
  (string/trim " foo bar "))