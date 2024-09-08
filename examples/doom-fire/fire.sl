(var width  (term/width) 
     height (term/height)
     pixels (resize [] (* width height) 0)
        
     max-y    (- height 1)
     max-fade 10

     avg-frames 0 
     avg-time  .0)

(^setup []
  (for [i (range 0 width)]
    (pixels i 255))

  (term/clear-screen)
  (term/move-to 1 1)
  (term/flush))

(^cleanup []
  (term/read-key)
  (term/reset)
  (term/clear-screen)
  (term/move-to 1 1)
  (term/flush)
  (term/restore))

(^render []
  (let [i (- 1)]
    (for [_ (range 0 max-y)]
      (for [x (range 0 width)]
        (let [j (+ i width)
              v (pixels (inc i))]
          (if (and (> x 0) (< x (- width 1)))
            (set j (+ j (- 1 (rand-int 3)))))
        
          (pixels j (- v (rand-int (min max-fade (+ v 1))))))))

  (set i (+ i width 1))
  (term/move-to 1 1)

  (for [_ (range 0 height)]
    (for [_ (range 0 width)]
      (let [g (pixels (dec i))
            r (else g 255 0)
            b (else (is g 255) 255 0)]
          (term/pick-back (rgb r g b))
          (term/write \\s)))
    (term/write \\n))

  (term/flush)))

(^run []  
  (loop
    (if (term/poll-key) (return)) 
    (render)))

(setup)
(run)
(cleanup)