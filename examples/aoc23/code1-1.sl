(lib aoc23-1-1)

(^find-digit [line]
  (char/digit (find-first:_ char/is-digit line)))

(^decode-line [line]
  (parse-int:_ (String (find-digit line) (find-digit (string/reverse line)))))

(^calibrate [input]
  (sum (map decode-line (io/read-lines input))))

(check 55108
  (calibrate "input1"))