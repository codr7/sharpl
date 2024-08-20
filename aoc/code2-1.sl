(lib aoc-23-2-1)

(^ decode-color [in out]
  (let [n:i (parse-int in)
        c (Sym (slice in (+ i 1)))]
    (out c (max (or (out c) 0) n))))
  
(^ decode-game [in out]
  (for [c (split in \,)]
    (decode-color c out)))
  
(^ decode-line [in]
  (let [games (split (slice in (+ (_:find-first (^[c] (is c \:)) in))) \;)]
    (reduce decode-game games {})))
	
(^ read-games [path]
  (enumerate (map decode-line (read-lines path)) 1))

(^ is-possible [game]
  (not (or (> (game 'red) 12)
           (> (game 'green) 13)
           (> (game 'blue) 14))))

(^ sum-games [path]
  (reduce + (map peek & peek (filter tail & is-possible (read-games path))) 0))
    
(check 2268
  (sum-games "input2"))