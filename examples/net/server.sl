(define ADDR (ARG 0))
(define PORT (parse-int:_ (ARG 1)))

(^handle-request [c]
  (let [req (c)]
    (say req)))

(define s (net/listen ADDR:PORT))
(define clients (List))

(loop
  (let [ready (poll s clients*)]
    (else (s is ready) 
      (new-client (s))
      (handle-request ready))))
 
(close s)