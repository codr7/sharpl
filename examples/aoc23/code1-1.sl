(lib aoc23-1-1)

(^find-digit [line]
  (char/digit (find-first:_ char/is-digit line)))

(^decode-line [line]
  (parse-int:_ (String (find-digit line) (find-digit (string/reverse line)))))

(^calibrate [path]
  (io/do-read [f path]
    (sum (map decode-line (io/lines f))*)))

(check 55108
  (calibrate "input1"))