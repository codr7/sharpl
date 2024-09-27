(lib aoc23-2)

(^read-color [in out]
  (let [n:i (parse-int in)
        c   (Sym (in i:_))]
    (out c (max (or (out c) 0) n))))
  
(^read-game [in out]
  (for [c (string/split in \,)]
    (read-color (string/trim c) out)))
  
(^read-line [in]
  (let [i     (_:find-first \: in)
        games (string/split (in (+ i 1):_) \;)]
    (reduce read-game games {})))
	
(^read-games [path]
  (io/do-read [f path]
    (enumerate 1 (map read-line (io/lines f)))))

(^is-possible [game]
  (not (or (> (game 'red) 12)
           (> (game 'green) 13)
           (> (game 'blue) 14))))

(^sum-games [path]
  (let [all-games (read-games path)
        possible-games (filter (^[g] (is-possible (g 1))) all-games)]
  (sum (map 0 possible-games)*)))
    
(check 2268
  (sum-games "input2"))