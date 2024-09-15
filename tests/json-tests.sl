(check (let [v {'foo:42 'bar:.5 'baz:"abc" 'qux:[T F _]}]
    (= (json/decode (json/encode v)) v)))