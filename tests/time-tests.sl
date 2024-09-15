(check (is (time/m 120) (time/h 2)))

(check (let [t (time/now)]
  (is (- (+ t (time/s 60)) (time/m))) t))

(check (let [t (time/now)]
  (is (- (+ t (time/d 7)) (time/w))) t))

(check (is (time/trunc (time/now)) (time/today)))

(check (let [d (time/m 90)
             t (+ (time/today) d)]
  (is (time/frac t) d)))
  
(check (let [t (time/now)]
  (is (time/to-local (time/to-utc t) t))))