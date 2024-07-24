(check 1
  1 (if 0 2))

(check 2
  1 (if 42 2))

(check 2
  (else 0 1 2))

(check 1
  (else 42 1 2))

(check 42
  (or F 42))

(check 0
  (or F 0))

(check (or T 42))