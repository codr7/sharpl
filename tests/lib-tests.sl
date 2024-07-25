(load "lib.sl")

(lib test
  (check test
    (lib))
    
  (var bar (+ foo 7)))

(check 42
  test/bar)

(check user
  (lib))