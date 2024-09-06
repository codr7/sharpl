(define width (term/width) 
        height (term/height) 
        max-y (- height 1)
        pixels (resize [] (* width height))
        max-fade 50
        avg-frames 0 
        avg-time .0)

  (fun init ()
    (for [i (range 0 width)]
      (set (# buf i) 0xff))

    (term/clear))

  (fun render ()
    (let [t0 (time/now) i -1]
      (for [_ (range 0 max-y)]
        (for [_ (range 0 (width x))]
          (let [v (pixels (inc i))
                j (+ i width)]
            (if (and x (< x (- width 1)))
              (inc j (- 1 (rand-int 3))))
        
            (pixels j (else v (- v (rand-int (min max-fade (+ (fix/to-int v) 1)))) v)))))

      (inc i (+ width 1))
      (term/home)
      
      (let [prev-g _]
        (for [_ (range 0 height)]
          (for [_ (range 0 width)]
            (let [g (pixels (dec i))
                  r (else g 0xff 0)
                  b (else (= g 0xff) 0xff 0)]
              (if (= g prev-g)
                (print out " ")
                (ctrl "48;2;" (int r) ";" (int (set prev-g g)) ";" (int b) "m "))))
          (say)))

      (term/flush)
      (inc avg-time (- (time/now) t0))
      (inc avg-frames)))

    (init)
    (for 50 (render))
    (term/reset)

(say (/ (* 1000000000.0 avg-frames) avg-time))