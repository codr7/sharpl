(check (char/is-digit \0))
(check F (char/is-digit \a))
(check 7 (char/digit \7))

(check (\a \a))
(check (not (\a \b)))

(check (= (char/up \a) \A))
(check (= (char/down \Z) \z))
