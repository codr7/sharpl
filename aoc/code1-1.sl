(lib aoc-1-1)

(^find-digit [line]
  (digit (find-first:_ char/is-digit line)))

(^decode-line [line]
  (parse-int:_ (String (find-digit line) (find-digit (string/reverse line)))))

(^calibrate [input]
  (reduce + (map decode-line (io/read-lines input)) 0))

(^solve []
  (check 55108
    (calibrate (path "input1"))))