(lib aoc23-2-1)

(^ decode-color [in out]
  (let [n:i (parse-int in)
        c   (sym (in (+ i 1):_))]
    (out c (max (or (out c) 0) n))))
  
(^ decode-game [in out]
  (for [c (split in \,)]
    (decode-color c out)))
  
(^ decode-line [in]
  (let [games (split (in (+ (_:find-first (^[c] (is c \:)) in)):_) \;)]
    (reduce decode-game games {})))
	
(^ read-games [path]
  (iter/enum (map decode-line (read-lines path)) 1))

(^ is-possible [game]
  (not (or (> (game 'red) 12)
           (> (game 'green) 13)
           (> (game 'blue) 14))))

(^ sum-games [path]
  (reduce + (map 0 (filter 1 & is-possible (read-games path))) 0))
    
(check 2268
  (sum-games "input2"))