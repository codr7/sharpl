(define width  (term/width) 
        height (term/height)
        pixels (resize [] (* width height))
        
        max-y    (- height 1)
        max-fade 50

        avg-frames 0 
        avg-time  .0)

(fun init ()
  (for [i (range 0 width)]
    (pixels i 255))

  (term/clear-screen)
  (term/move-to 1 1)
  (term/flush))

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
  (term/move-to 1 1)
      
  (let [prev-g _]
    (for [_ (range 0 height)]
      (for [_ (range 0 width)]
        (let [g (pixels (dec i))
              r (else g 255 0)
              b (else (= g 255) 255 0)]
          (if (not (is g prev-g))
            (term/pick-bg (rgb (fix/to-int r) (fix/to-int g) (fix/to-int b)))
            (term/write " ")
            (set prev-g g))))
      (term/write \\n)))

  (term/flush)
  
  (inc avg-time (- (time/now) t0))
  (inc avg-frames)))

(^run []  
  (for [_ (range 0 50)]
    (if (term/poll-key) (return)) 
    (render)))

(init)
(run)

(term/reset)
(term/clear-screen)
(term/move-to 1 1)
(term/flush)
  
(say (/ (* 1000000000.0 avg-frames) avg-time))