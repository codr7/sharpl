(check "{\"bar\": [true,false,null],\"baz\": 0.5,\"foo\": 42}"
  (json/encode {'foo: 42 "bar": [T F _] 'baz:.5}))

(check _
  (json/decode " "))

(check 42
  (json/decode " 42"))

(check 1.5
  (json/decode "1.5"))

(check T
  (json/decode "true"))

(check F
  (json/decode "false"))

(check _
  (json/decode "null"))