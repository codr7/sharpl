(let [s (net/listen (ARG 0):(parse-int:_ (ARG 1)))]
  (for [c s]
    (say c)
    (close c))
  (close s))