(check ["foo" "bar" "baz"]
  (io/do-read [in "input.txt"] 
    [(io/lines in)*]))

(check ["foo" "bar" "baz"]
  [(io/read-lines "input.txt")*])