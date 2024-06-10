(check 42 
  42)

(check 42
  (do 42))

(check 42
  (define foo 42)
  foo)

(check 42
  (let [x 42] x))

(check 3
  (+ 1 2))
  
(check 2
  (- 3 1))

(check (- 2)
  (- 1 3))

(check 2
  (let [x [1 2 3]]
    (x 1)))
    
(check [1 4 3]
  (let [x [1 2 3]]
    (x 1 4)
    x))

(check "FOO"
  (string/up "Foo"))

(check "foo"
  (string/down "Foo"))



(load "test-lib.sl")

(lib test
  (check test
    (lib))
    
  (define bar (+ foo 7)))

(check 42
  test/bar)

(check user
  (lib))