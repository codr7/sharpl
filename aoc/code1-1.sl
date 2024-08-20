(lib aoc-23-1-1)

(^find-digit [line]
  (char/digit (find-first:_ char/is-digit line)))

(^decode-line [line]
  (parse-int:_ (String (find-digit line) (find-digit (string/reverse line)))))

(^calibrate [input]
  (reduce + (map decode-line (io/read-lines input)) 0))

(check 55108
  (calibrate "input1"))