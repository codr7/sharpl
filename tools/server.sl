(let [s (net/listen (ARG 0) (parse-int (ARG 1)))]
  (say s)
  (close s))