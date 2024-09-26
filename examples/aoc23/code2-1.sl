(lib aoc23-2-1)

(^decode-color [in out]
  (let [n:i (parse-int in)
        c   (Sym (in i:_))]
    (out c (max (or (out c) 0) n))))
  
(^decode-game [out in]
  (for [c (string/split in ",")]
    (decode-color (string/trim c) out)))
  
(^decode-line [in]
  (let [i     (_:find-first (^[c] (is c \:)) in)
        games (string/split (in (+ i 1):_) ";")]
    (reduce decode-game games {})))
	
(^read-games [path]
  (io/do-read [f path]
    (enumerate 1 (map decode-line (io/lines f)))))

(^is-possible [game]
  (not (or (> (game 'red) 12)
           (> (game 'green) 13)
           (> (game 'blue) 14))))

(^sum-games [path]
  (sum (map 0 (filter (^[g] (is-possible (g 1))) (read-games path)))*))
    
(check 2268
  (sum-games "input2"))