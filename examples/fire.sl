(define width 50 
        height 25 
        max-y (- height 1)
        buf (Bin/new (* width height))
        max-fade 50
        avg-frames 0 
        avg-time .0)

  (fun init ()
    (for (width i)
      (set (# buf i) 0xff))

    (term/clear))

  (fun render ()
    (let t0 (now) i -1)

    (for max-y
      (for (width x)
        (let v (# buf (inc i))
             j (+ i width))
        
        (if (and x (< x (- width 1)))
          (inc j (- 1 (rand 3))))
        
        (set (# buf j)
             (if v (- v (rand (min max-fade (+ (int v) 1)))) v))))

    (inc i (+ width 1))
    (let prev-g _)
    (home)
    
    (for height
      (for width
        (let g (# buf (dec i))
             r (if g 0xff 0)
             b (if (= g 0xff) 0xff 0))
             
        (if (= g prev-g)
          (print out " ")
          (ctrl "48;2;" (int r) ";" (int (set prev-g g)) ";" (int b) "m ")))

      (print out \n))

    (flush out)
    (inc avg-time (- (now) t0))
    (inc avg-frames))

(init)
(for 50 (render))
(term/reset)

(say (/ (* 1000000000.0 fire/avg-frames) fire/avg-time))