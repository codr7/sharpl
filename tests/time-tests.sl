(check (is (time/m 120) (time/h 2)))

(check (let [t (time/now)]
  (is (time/to-local (time/to-utc t) t))))