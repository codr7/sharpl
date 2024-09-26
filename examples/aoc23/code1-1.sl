(lib aoc23-1)

(^find-digit [line]
  (char/digit (find-first:_ char/is-digit line)))

(^read-line [line]
  (parse-int:_ (String (find-digit line) (find-digit (string/reverse line)))))

(^calibrate [path]
  (io/do-read [f path]
    (sum (map read-line (io/lines f))*)))

(check 55108
  (calibrate "input1"))