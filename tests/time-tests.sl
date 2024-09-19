(let [t (Timestamp 2024 9 16 21 22 23)]
  (check (is (time/Y t) 2024))
  (check (is (time/M t) 9))
  (check (is (time/D t) 16))
  (check (is (time/h t) 21))
  (check (is (time/m t) 22))
  (check (is (time/s t) 23))
  (check (is (time/ms t) 0))
  (check (is (time/us t) 0)))

(check (is (time/m 120) (time/h 2)))

(check (let [t (time/now)]
  (is (- (+ t (time/s 60)) (time/m))) t))

(check (let [t (time/now)]
  (is (- (+ t (time/D 7)) (time/W))) t))

(check (let [t (time/now)]
  (is (- (+ t (time/M 12)) (time/Y))) t))

(check (is (time/trunc (time/now)) (time/today)))

(check (let [d (time/m 90)
             t (+ (time/today) d)]
  (is (time/frac t) d)))

(check 4 
  (let [t (Timestamp 2024 1 1)]
    #[t..(+ t (time/D 1)):(time/h 6)*]))

(let [t (Timestamp 2024 9 19)]
  (check 'sep (time/MONTHS (time/M t)))
  (check 'th (time/WEEKDAYS (time/WD t))))

(check (let [t (time/now)]
  (is (time/to-local (time/to-utc t) t))))