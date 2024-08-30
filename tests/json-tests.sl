(check "{\"bar\": [1,2,3],\"baz\": 0.5,\"foo\": 42,\"qux\": true}"
  (json/encode {'foo: 42 "bar": [1 2 3] 'baz:.5 'qux: T}))