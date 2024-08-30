(check "{\"bar\": [1,2,3],\"baz\": 0.5,\"foo\": 42,\"qux\": true}"
  (json/encode {'foo: 42 "bar": [1 2 3] 'baz:.5 'qux: T}))

(check 42
  (json/decode "42"))

(check 1.5
  (json/decode "1.5"))