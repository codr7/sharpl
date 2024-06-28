(check "AbAAbbA"
  (rxplace "abaabba" "a" "A"))

(check "foo-bar"
  (rxplace "foo  bar" "(\w+) (\w+)" "$1-$2"))